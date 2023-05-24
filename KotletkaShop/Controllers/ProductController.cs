using KotletkaShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["VendorSortParm"] = sortOrder == "Vendor" ? "vendor_desc" : "Vendor";
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Models.Product> products = from s in _context.Products
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

            products = sortOrder switch
            {
                "title_desc" => products.OrderByDescending(s => s.Title),
                "Quantity" => products.OrderBy(s => s.Quantity),
                "quantity_desc" => products.OrderByDescending(s => s.Quantity),
                "Type" => products.OrderBy(s => s.ProductType),
                "type_desc" => products.OrderByDescending(s => s.ProductType),
                "Vendor" => products.OrderBy(s => s.Vendor),
                "vendor_desc" => products.OrderByDescending(s => s.Vendor),
                _ => products.OrderBy(s => s.Title),
            };

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
            Models.Product product = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.Image)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ProductID == id);

            return product == null ? NotFound() : View(product);
        }
    }
}
