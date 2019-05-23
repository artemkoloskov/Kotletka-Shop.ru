using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly StoreContext _context;

        public ProductsController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Иницализация главного представления списка товаров для админки.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // загрузка из контекста списка товаров.
            List<Product> products = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.Image)
                .AsNoTracking()
                .ToListAsync();

            return View(products);
        }

        /// <summary>
        /// Инициализация представления деталей конкретного товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загрузка товара из БД по идентификатору
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

            // Загрузка коллекций из БД
            List<Collection> collections = await _context.Collections
                .Include(c => c.Image)
                .AsNoTracking()
                .ToListAsync();

            // проверка коллеций на факт содержания текущего товара
            foreach (Collection c in collections)
            {
                c.Products = await c.GetCollectionProducts(_context);

                if (c.ProductsListContains(id))
                {
                    // Если коллекция содержит данный товар - записываем ее в 
                    // список коллекций, в которых состоит товар, для отображения
                    // в деталях товара
                    product.Collections.Add(c);
                }
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Handle,Title,Body,Vendor,ProductTypeID,Tags," +
                "Published,Option1Name,Option1Value,Option2Name,Option2Value," +
                "Option3Name,Option3Value,Weight,Quantity,Price," +
                "VisibleFrom,VisibleUntil")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
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

            return View(product);
        }
    }
}
