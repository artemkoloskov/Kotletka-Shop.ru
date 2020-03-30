﻿using System.Collections.Generic;
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
                "VisibleFrom,VisibleUntil,ProductImages")] Product product)
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

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Edit/5
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

            return View(productToUpdate);
        }

        // GET: Products/Delete/5
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
                _context.Products.Remove(product);
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
