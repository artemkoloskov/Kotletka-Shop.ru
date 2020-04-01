using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly StoreContext _context;

        public OrdersController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Иницализация главного представления списка заказов для админки.
        /// </summary>
        /// <param name="sortOrder">Параметр сортировки</param>
        /// <param name="searchString">Параметр поиска</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PaidSortParm"] = sortOrder == "Paid" ? "paid_desc" : "Paid";
            ViewData["FulfilledSortParm"] = sortOrder == "Fulfilled" ? "fulfilled_desc" : "Fulfilled";
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Order> orders = from s in _context.Orders
                           select s;

            // Если строка поиска не пустая - пробуем считать из строки целое число
            // и выполняем поиск по нему, или, если число не удалось счтать,
            // ищем по самой строке
            // Поиск осуществляется по идентификтору клиента, идентификтаору заказа,
            // адресу клиента, имени клиента, фамилии клиента, отчеству клиента,
            // имейлу, заметкам по клиенту, номеру телефона, тэгам клиента
            if (!string.IsNullOrEmpty(searchString))
            {

                orders = int.TryParse(searchString, out int searchNumber)
                    ? orders.Where(o => o.CustomerID == searchNumber
                                    || o.OrderID == searchNumber
                                    || o.Customer.Apartment.Contains(searchString)
                                    || o.Customer.FirstName.Contains(searchString)
                                    || o.Customer.LastName.Contains(searchString)
                                    || o.Customer.MiddleName.Contains(searchString)
                                    || o.Customer.City.Contains(searchString)
                                    || o.Customer.Country.Contains(searchString)
                                    || o.Customer.District.Contains(searchString)
                                    || o.Customer.Email.Contains(searchString)
                                    || o.Customer.Note.Contains(searchString)
                                    || o.Customer.PhoneNumber.Contains(searchString)
                                    || o.Customer.Province.Contains(searchString)
                                    || o.Customer.Street.Contains(searchString)
                                    || o.Customer.Tags.Contains(searchString)
                                )
                    : orders.Where(o => o.Note.Contains(searchString)
                                    || o.Customer.Apartment.Contains(searchString)
                                    || o.Customer.FirstName.Contains(searchString)
                                    || o.Customer.LastName.Contains(searchString)
                                    || o.Customer.MiddleName.Contains(searchString)
                                    || o.Customer.City.Contains(searchString)
                                    || o.Customer.Country.Contains(searchString)
                                    || o.Customer.District.Contains(searchString)
                                    || o.Customer.Email.Contains(searchString)
                                    || o.Customer.Note.Contains(searchString)
                                    || o.Customer.PhoneNumber.Contains(searchString)
                                    || o.Customer.Province.Contains(searchString)
                                    || o.Customer.Street.Contains(searchString)
                                    || o.Customer.Tags.Contains(searchString)
                                );
            }

            // Перебор сортировки на основе параметра поиска.
            orders = sortOrder switch
            {
                "id_desc" => orders.OrderByDescending(s => s.OrderID),
                "Date" => orders.OrderBy(s => s.DateCreated),
                "date_desc" => orders.OrderByDescending(s => s.DateCreated),
                "Name" => orders.OrderBy(s => s.Customer.LastName),
                "name_desc" => orders.OrderByDescending(s => s.Customer.LastName),
                "Paid" => orders.OrderBy(s => s.DateCreated),
                "paid_desc" => orders.OrderByDescending(s => s.DateCreated),
                "Fulfilled" => orders.OrderBy(s => s.DateCreated),
                "fulfilled_desc" => orders.OrderByDescending(s => s.DateCreated),
                _ => orders.OrderBy(s => s.Customer.LastName),
            };

            // загрузка из контекста списка закозов. НЕ ЧЕРНОВИКИ
            return View(await orders.
                Include(o => o.Customer).
                Include(o => o.OrderProducts).
                    ThenInclude(op => op.Product).
                Include(o => o.OrderDiscounts).
                    ThenInclude(od => od.Discount).
                Where(o => o.Draft == false).
                AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Инициализация представления деталей конкретного заказа
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //  загрузка заказа из БД по идентификатору
            Order order = await _context.Orders
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Orders)
                .Include(o => o.OrderProducts)
                    .ThenInclude(p => p.Product)
                .Include(o => o.OrderDiscounts)
                    .ThenInclude(d => d.Discount)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
