using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class CustomersController : Controller
    {
        private readonly StoreContext _context;

        public CustomersController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Инициализация представления списка клиентов
        /// </summary>
        /// <param name="sortOrder">Параметр сортировки</param>
        /// <param name="searchString">Параметр поиска</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Customer> customers = from s in _context.Customers
                                             select s;

            // Если строка поиска не пустая - пробуем считать из строки целое число
            // и выполняем поиск по нему, или, если число не удалось счтать,
            // ищем по самой строке
            // Поиск осушествляется по имени, фамилии, адресу, имейлу, заметкам,
            // номеру телефона, тэгам, индексу,
            if (!string.IsNullOrEmpty(searchString))
            {

                customers = int.TryParse(searchString, out int searchNumber)
                    ? customers.Where(c => c.FirstName.Contains(searchString)
                                                   || c.LastName.Contains(searchString)
                                                   || c.MiddleName.Contains(searchString)
                                                   || c.City.Contains(searchString)
                                                   || c.Country.Contains(searchString)
                                                   || c.District.Contains(searchString)
                                                   || c.Email.Contains(searchString)
                                                   || c.Note.Contains(searchString)
                                                   || c.PhoneNumber.Contains(searchString)
                                                   || c.Province.Contains(searchString)
                                                   || c.Street.Contains(searchString)
                                                   || c.Tags.Contains(searchString)
                                                   || c.ZipCode == searchNumber
                                                )
                    : customers.Where(c => c.FirstName.Contains(searchString)
                                                   || c.LastName.Contains(searchString)
                                                   || c.MiddleName.Contains(searchString)
                                                   || c.City.Contains(searchString)
                                                   || c.Country.Contains(searchString)
                                                   || c.District.Contains(searchString)
                                                   || c.Email.Contains(searchString)
                                                   || c.Note.Contains(searchString)
                                                   || c.PhoneNumber.Contains(searchString)
                                                   || c.Province.Contains(searchString)
                                                   || c.Street.Contains(searchString)
                                                   || c.Tags.Contains(searchString)
                                                );

            }

            customers = sortOrder switch
            {
                "name_desc" => customers.OrderByDescending(s => s.LastName),
                _ => customers.OrderBy(s => s.LastName),
            };

            return View(await customers
                        .Include(c => c.Orders)
                        .Include(c => c.Payments)
                        .Include(c => c.Image)
                        .AsNoTracking()
                        .ToListAsync());
        }

        /// <summary>
        /// Инициализация представления деталей конкретного клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загрузка из БД сущности клиента, вместе с его заказами, платежами
            // и фото профиля
            Customer customer = await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Payments)
                .Include(c => c.Image)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.CustomerID == id);

            return customer == null ? NotFound() : View(customer);
        }

        // GET: Admin/Customers/Create
        /// <summary>
        /// Инициализация представления создания нового клиента.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create
        /// <summary>
        /// Инициализация представления создания нового клиента.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FirstName,MiddleName,LastName,Country,City,Province," +
                "District,Street,Building,Apartment,ZipCode," +
                "PhoneNumber,Email,Note,AcceptsMarketing,RegisterDate," +
                "ImageID,Tags")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _ = _context.Add(customer);
                    _ = await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Не удалось сохранить изменения. " +
                    "Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.");
            }

            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        /// <summary>
        /// Инициализация представления редактирования клиента
        /// </summary>
        /// <param name="id">Идентификатор редактируемого клиента</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer customer = await _context.Customers.FindAsync(id);
            return customer == null ? NotFound() : View(customer);
        }

        // POST: Admin/Customers/Edit/5
        /// <summary>
        /// Инициализация представления редактирования клиента
        /// </summary>
        /// <param name="id">Идентификатор редактируемого клиента</param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer customerToUpdate = await _context.Customers.FirstOrDefaultAsync(s => s.CustomerID == id);

            if (await TryUpdateModelAsync<Customer>(
                customerToUpdate,
                "",
                c => c.FirstName, c => c.AcceptsMarketing, c => c.Apartment,
                c => c.Building, c => c.City, c => c.Country, c => c.District,
                c => c.Email, c => c.FirstName, c => c.ImageID, c => c.LastName,
                c => c.MiddleName, c => c.Note, c => c.PhoneNumber,
                c => c.Province, c => c.Street, c => c.Tags, c => c.ZipCode))
            {
                try
                {
                    _ = await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Не удалось сохранить изменения. " +
                    "Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.");
                }
            }

            return View(customerToUpdate);
        }

        // GET: Admin/Customers/Delete/5
        /// <summary>
        /// Инициализация представления удаления клиента
        /// </summary>
        /// <param name="id">Идентификтор удаляемого клиента</param>
        /// <param name="saveChangesError">Маркер ошибки удаления</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Удаление не удалось. Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.";
            }

            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _ = _context.Customers.Remove(customer);
                _ = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), (id, saveChangesError: true));
            }
        }
    }
}
