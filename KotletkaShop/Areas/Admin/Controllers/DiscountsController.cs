using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
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
            ViewData["HandleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "handle_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Discount> discounts = from s in _context.Discounts
                                             select s;

            // Если строка поиска не пустая - пробуем считать из строки целое число
            // и выполняем поиск по нему, или, если число не удалось счтать,
            // ищем по самой строке
            if (!string.IsNullOrEmpty(searchString))
            {

                discounts = int.TryParse(searchString, out int searchNumber)
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загрузка скидки из БД по идентификатору
            Discount discount = await _context.Discounts
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

            return discount == null ? NotFound() : (IActionResult)View(discount);
        }

        /// <summary>
        /// Загружает список клиентов, которые могут воспользоваться скидкой
        /// </summary>
        /// <returns>Список клиентов</returns>
        /// <param name="ids">Список идентификаторов по которым будет сделан поиск по базе данных.</param>
        private async Task<List<Customer>> GetEligibleCustomers(List<int> ids)
        {
            List<Customer> customers = new List<Customer>();

            foreach (int id in ids)
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
            List<Collection> collections = new List<Collection>();

            foreach (int id in ids)
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
            List<Product> products = new List<Product>();

            foreach (int id in ids)
            {
                products.Add(await _context.Products.Where(c => c.ProductID == id).SingleOrDefaultAsync());
            }

            return products;
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collections/Create
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
                    _context.Add(discount);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Discount discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        // POST: Admin/Discounts/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Discount discountToUpdate = await _context.Discounts.FirstOrDefaultAsync(s => s.DiscountID == id);

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
                    await _context.SaveChangesAsync();
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

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Discount discount = await _context.Discounts
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

        // POST: Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Discount discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), (id, saveChangesError: true));
            }
        }
    }
}
