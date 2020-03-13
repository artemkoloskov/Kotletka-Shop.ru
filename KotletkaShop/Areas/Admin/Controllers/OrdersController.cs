using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.EntityFrameworkCore;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PaidSortParm"] = sortOrder == "Paid" ? "paid_desc" : "Paid";
            ViewData["FulfilledSortParm"] = sortOrder == "Fulfilled" ? "fulfilled_desc" : "Fulfilled";

            IQueryable<Order> orders = from s in _context.Orders
                           select s;

            switch (sortOrder)
            {
                case "id_desc":
                    orders = orders.OrderByDescending(s => s.OrderID);
                    break;
                case "Date":
                    orders = orders.OrderBy(s => s.DateCreated);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.DateCreated);
                    break;
                case "Name":
                    orders = orders.OrderBy(s => s.Customer.LastName);
                    break;
                case "name_desc":
                    orders = orders.OrderByDescending(s => s.Customer.LastName);
                    break;
                case "Paid":
                    orders = orders.OrderBy(s => s.DateCreated);
                    break;
                case "paid_desc":
                    orders = orders.OrderByDescending(s => s.DateCreated);
                    break;
                case "Fulfilled":
                    orders = orders.OrderBy(s => s.DateCreated);
                    break;
                case "fulfilled_desc":
                    orders = orders.OrderByDescending(s => s.DateCreated);
                    break;
                default:
                    orders = orders.OrderBy(s => s.Customer.LastName);
                    break;
            }

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
