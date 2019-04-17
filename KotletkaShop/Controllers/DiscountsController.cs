using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<IActionResult> Index()
        {
            List<Models.Discount> discounts = await _context.Discounts.AsNoTracking().ToListAsync();

            return View(discounts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
                customers.Add(await _context.Customers.Where(c => c.CustomerID == id).SingleOrDefaultAsync());
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
    }
}
