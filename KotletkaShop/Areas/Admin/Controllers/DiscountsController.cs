using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class DiscountsController : Controller
    {
        private readonly StoreContext _context;

        public DiscountsController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Инициализация представления списка скидок
        /// </summary>
        /// <param name="sortOrder">Параметр сортировки</param>
        /// <param name="searchString">Параметр поиска по списку</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["HandleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "handle_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var discounts = from s in _context.Discounts
                            select s;

            // Если строка поиска не пустая - пробуем считать из строки целое число
            // и выполняем поиск по нему, или, если число не удалось счтать,
            // ищем по самой строке
            // Поиск осуществляется по названию, величине скидки, номеру идентивфикатора
            // и минимальномк значению
            if (!string.IsNullOrEmpty(searchString))
            {

                discounts = int.TryParse(searchString, out var searchNumber)
                    ? discounts.Where(d => d.Handle.Contains(searchString)
                                        || d.Value == searchNumber
                                        || d.DiscountID == searchNumber
                                        || d.MinimumRequirementValue == searchNumber
                                    )
                    : discounts.Where(d => d.Handle.Contains(searchString));
            }

            discounts = sortOrder switch
            {
                "handle_desc" => discounts.OrderByDescending(s => s.Handle),
                _ => discounts.OrderBy(s => s.Handle),
            };

            return View(await discounts.AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Инициализация представления деталей конкретной скидки
        /// </summary>
        /// <param name="id">Идентификатор скидки</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загрузка скидки из БД по идентификатору
            var discount = await _context.Discounts
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.DiscountID == id);

            if (discount == null)
            {
                return NotFound();
            }

            // Загружаем список товаров или коллекций, к которым применима скидка.
            if (discount.AppliesTo == DiscountApplicableObjectTypes.SpecificProducts)
            {
                discount.ApplicableProducts = await GetApplicableProducts(discount.ApplyableObjectsIDs);
            }
            else if (discount.AppliesTo == DiscountApplicableObjectTypes.SpecificCollections)
            {
                discount.ApplicableCollections = await GetApplicableCollections(discount.ApplyableObjectsIDs);
            }

            // Загружаем список клиентов, которые  могут воспользоваться скидкой.
            if (discount.CustomerEligibility == DiscountEligibleObjectTypes.SpecificCustomers)
            {
                discount.EligibleCustomers = await GetEligibleCustomers(discount.EligibleObjectsIDs);
            }

            return discount == null ? NotFound() : View(discount);
        }

        /// <summary>
        /// Загружает список клиентов, которые могут воспользоваться скидкой
        /// </summary>
        /// <returns>Список клиентов</returns>
        /// <param name="ids">Список идентификаторов по которым будет сделан поиск по базе данных.</param>
        private async Task<List<Customer>> GetEligibleCustomers(List<int> ids)
        {
            var customers = new List<Customer>();

            foreach (var id in ids)
            {
                customers.Add(await _context.Customers.Include(c => c.Image).Where(c => c.CustomerID == id).SingleOrDefaultAsync());
            }

            return customers;
        }

        /// <summary>
        /// Загружает список коллекций, к которым применима скидка
        /// </summary>
        /// <returns>Список коллекций.</returns>
        /// <param name="ids">Идентификаторы коллекций, по которым будет сделан поиск по базе данных.</param>
        private async Task<List<Collection>> GetApplicableCollections(List<int> ids)
        {
            var collections = new List<Collection>();

            foreach (var id in ids)
            {
                collections.Add(await _context.Collections.Where(c => c.CollectionID == id).SingleOrDefaultAsync());
            }

            return collections;
        }

        /// <summary>
        /// Загружает из БД список товаров, к которым применима скидка
        /// </summary>
        /// <returns>Список товаоров, к которым применима скидка</returns>
        /// <param name="ids">Идентификаторы товаров, по которым будет сделан поиск по базе данных.</param>
        private async Task<List<Product>> GetApplicableProducts(List<int> ids)
        {
            var products = new List<Product>();

            foreach (var id in ids)
            {
                products.Add(await _context.Products.Where(c => c.ProductID == id).SingleOrDefaultAsync());
            }

            return products;
        }

        // GET: Admin/Customers/Create
        /// <summary>
        /// Инициализация представления создания новой скидки
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Collections/Create
        /// <summary>
        /// Инициализация представления создания новой скидки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Handle,Type,Value,AppliesTo,ApplicableObjects," +
                "MinimumRequirement,MinimumRequirementValue," +
                "CustomerEligibility,EligibleObjects,StartDate," +
                "EndDate,IsActive,MaxTimesUsed,OneUsePerCustomer," +
                "TimesUsed")] Discount discount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _ = _context.Add(discount);
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

            return View(discount);
        }

        // GET: Admin/Discounts/Edit/5
        /// <summary>
        /// Инициализация представления редактирования конкретной скидки
        /// </summary>
        /// <param name="id">Идентификатор скидки</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts.FindAsync(id);
            return discount == null ? NotFound() : View(discount);
        }

        // POST: Admin/Discounts/Edit/5
        /// <summary>
        /// Инициализация представления редактирования конкретной скидки
        /// </summary>
        /// <param name="id">Идентификатор скидки</param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountToUpdate = await _context.Discounts.FirstOrDefaultAsync(s => s.DiscountID == id);

            if (await TryUpdateModelAsync<Discount>(
                discountToUpdate,
                "",
                c => c.ApplicableObjects, c => c.AppliesTo,
                c => c.CustomerEligibility, c => c.EligibleObjects, c => c.EndDate,
                c => c.Handle, c => c.IsActive, c => c.MaxTimesUsed,
                c => c.MinimumRequirement, c => c.MinimumRequirementValue,
                c => c.OneUsePerCustomer, c => c.StartDate, c => c.TimesUsed,
                c => c.Type, c => c.Value))
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

            return View(discountToUpdate);
        }

        // GET: Admin/Discounts/Delete/5
        /// <summary>
        /// инциализация представления удаления скидки 
        /// </summary>
        /// <param name="id">Идентификтор скидки</param>
        /// <param name="saveChangesError">Маркер ошибки удаления</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DiscountID == id);
            if (discount == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Удаление не удалось. Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.";
            }

            return View(discount);
        }

        // POST: Admin/Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _ = _context.Discounts.Remove(discount);
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
