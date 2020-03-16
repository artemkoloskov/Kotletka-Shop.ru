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
    public class ProductController : Controller
    {
        private readonly StoreContext _context;

        public ProductController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Иницализация страницы со списком всех товаров.
        /// </summary>
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["VendorSortParm"] = sortOrder == "Vendor" ? "vendor_desc" : "Vendor";
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Product> products = from s in _context.Products
                                           select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = int.TryParse(searchString, out int searchNumber)
                    ? products.Where(p => p.Title.Contains(searchString)
                                       || p.Body.Contains(searchString)
                                       || p.Option1Name.Contains(searchString)
                                       || p.Option2Name.Contains(searchString)
                                       || p.Option3Name.Contains(searchString)
                                       || p.Tags.Contains(searchString)
                                       || p.Vendor.Contains(searchString)
                                       || p.Price == searchNumber
                                       || p.Quantity == searchNumber
                                       || p.Weight == searchNumber
                                       )
                    : products.Where(p => p.Title.Contains(searchString)
                                       || p.Body.Contains(searchString)
                                       || p.Option1Name.Contains(searchString)
                                       || p.Option2Name.Contains(searchString)
                                       || p.Option3Name.Contains(searchString)
                                       || p.Tags.Contains(searchString)
                                       || p.Vendor.Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "title_desc":
                    products = products.OrderByDescending(s => s.Title);
                    break;
                case "Quantity":
                    products = products.OrderBy(s => s.Quantity);
                    break;
                case "quantity_desc":
                    products = products.OrderByDescending(s => s.Quantity);
                    break;
                case "Type":
                    products = products.OrderBy(s => s.ProductType);
                    break;
                case "type_desc":
                    products = products.OrderByDescending(s => s.ProductType);
                    break;
                case "Vendor":
                    products = products.OrderBy(s => s.Vendor);
                    break;
                case "vendor_desc":
                    products = products.OrderByDescending(s => s.Vendor);
                    break;
                default:
                    products = products.OrderBy(s => s.Title);
                    break;
            }

            // загрузка из контекста списка товаров.
            return View(await products
                            .Include(p => p.ProductType)
                            .Include(p => p.ProductImages)
                                .ThenInclude(pi => pi.Image)
                            .AsNoTracking()
                            .ToListAsync()
                            );
        }

        /// <summary>
        /// Инициализация представления страницы конкретного товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        public async Task<IActionResult> Product(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // загрузка коллекции из БД по идентификтору
            Product product = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.Image)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
