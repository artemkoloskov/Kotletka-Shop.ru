﻿using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
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
        /// <param name="sortOrder">Параметр сортировки</param>
        /// <param name="searchString">Параметр поиска</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
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
                                       || p.Handle.Contains(searchString)
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
                                       || p.Handle.Contains(searchString)
                                       || p.Body.Contains(searchString)
                                       || p.Option1Name.Contains(searchString)
                                       || p.Option2Name.Contains(searchString)
                                       || p.Option3Name.Contains(searchString)
                                       || p.Tags.Contains(searchString)
                                       || p.Vendor.Contains(searchString)
                                       );
            }

            // Перебор параметра сортировки
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
                .ToListAsync());
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
            System.Collections.Generic.List<Collection> collections = await _context.Collections
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

        // GET: Admin/Products/Create
        /// <summary>
        /// Инициализация представления создания нового товара
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Products/Create
        /// <summary>
        /// Ининциализация представления создания нового товара
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Handle,Title,Body,Vendor,ProductTypeID,Tags," +
                "Published,Option1Name,Option1Value,Option2Name,Option2Value," +
                "Option3Name,Option3Value,Weight,Quantity,Price," +
                "VisibleFrom,VisibleUntil,ProductImages")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _ = _context.Add(product);
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

            return View(product);
        }

        // GET: Admin/Products/Edit/5
        /// <summary>
        /// Инициализация представления редактирования товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.FindAsync(id);

            return product == null ? NotFound() : View(product);
        }

        // POST: Admin/Products/Edit/5
        /// <summary>
        /// Инициализация представления редактирования товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product productToUpdate = await _context.Products.FirstOrDefaultAsync(s => s.ProductID == id);

            if (await TryUpdateModelAsync<Product>(
                productToUpdate,
                "",
                c => c.Handle, c => c.Title, c => c.Body, c => c.Vendor,
                c => c.ProductTypeID, c => c.Tags, c => c.Published,
                c => c.Option1Name, c => c.Option1Value,
                c => c.Option2Name, c => c.Option2Value,
                c => c.Option3Name, c => c.Option3Value, c => c.Weight,
                c => c.Quantity, c => c.Price, c => c.VisibleFrom,
                c => c.VisibleUntil))
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

            return View(productToUpdate);
        }

        // GET: Products/Delete/5
        /// <summary>
        /// Инициализация представления удаления товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <param name="saveChangesError"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Удаление не удалось. Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.";
            }

            return View(product);
        }

        // POST: Products/Delete/5
        /// <summary>
        /// Инициализация представления удаления товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <param name="saveChangesError"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _ = _context.Products.Remove(product);
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
