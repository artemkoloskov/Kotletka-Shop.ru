using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static KotletkaShop.Models.CollectionCompareSubjects;
using static KotletkaShop.Models.CollectionConditions;
using static KotletkaShop.Models.SortingOrders;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    public class CollectionsController : Controller
    {
        private readonly StoreContext _context;

        public CollectionsController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Инициализация главного пердставления списка коллекций товаров
        /// </summary>
        /// <returns>The index.</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collections.ToListAsync());
        }

        /// <summary>
        /// Инициализация представления деталей конкретной коллекции
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Collection collection = await _context.Collections
                .SingleOrDefaultAsync(m => m.CollectionID == id);

            if (collection == null)
            {
                return NotFound();
            }

            collection.Products = await GetCollectionProducts(collection);

            return View(collection);
        }

        /// <summary>
        /// Поиск продуктов для коллекции
        /// </summary>
        /// <returns>Список продуктов</returns>
        /// <param name="coll">Коллекция для которой строится список.</param>
        private async Task<List<Product>> GetCollectionProducts(Collection coll)
        {
            List<Product> products = new List<Product>();

            if (coll.MatchAllConditions)
            {
                if (coll.CompareSubject1 > 0 && coll.Condition1 > 0 && coll.CompareTo1 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(coll.CompareSubject1, coll.Condition1, coll.CompareTo1));
                }
                if (coll.CompareSubject2 > 0 && coll.Condition2 > 0 && coll.CompareTo2 != "")
                {
                    products = ApplyNewConditionsToProductList(products, coll.CompareSubject2, coll.Condition2, coll.CompareTo2);
                }
                if (coll.CompareSubject3 > 0 && coll.Condition3 > 0 && coll.CompareTo3 != "")
                {
                    products = ApplyNewConditionsToProductList(products, coll.CompareSubject3, coll.Condition3, coll.CompareTo3);
                }

            }
            else
            {
                if (coll.CompareSubject1 > 0 && coll.Condition1 > 0 && coll.CompareTo1 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(coll.CompareSubject1, coll.Condition1, coll.CompareTo1));
                }
                if (coll.CompareSubject2 > 0 && coll.Condition2 > 0 && coll.CompareTo2 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(coll.CompareSubject2, coll.Condition2, coll.CompareTo2));
                }
                if (coll.CompareSubject3 > 0 && coll.Condition3 > 0 && coll.CompareTo3 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(coll.CompareSubject3, coll.Condition3, coll.CompareTo3));
                }
            }

            return products;
        }

        /// <summary>
        /// Поиск продуктов по условиям коллекции
        /// </summary>
        /// <returns>Список продуктов</returns>
        /// <param name="compareSubject">Субъект сравнения</param>
        /// <param name="condition">Условие для проверки</param>
        /// <param name="compareTo">Объект сравнения</param>
        private async Task<List<Product>> GetCollectionProductsByConditions(CollectionCompareSubjects compareSubject, CollectionConditions condition, string compareTo)
        {
            List<Product> products = new List<Product>();

            if (compareSubject == ProductTag)
            {
                switch (condition)
                {
                    case IsEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Tags.Contains(compareTo)).ToListAsync());
                        break;
                }
            }
            else if (compareSubject == ProductPrice)
            {
                switch (condition)
                {
                    case IsEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Price.Equals(double.Parse(compareTo))).ToListAsync());
                        break;
                    case IsGreaterThan:
                        products.AddRange(await _context.Products.Where(p => p.Price > double.Parse(compareTo)).ToListAsync());
                        break;
                    case IsLessThen:
                        products.AddRange(await _context.Products.Where(p => p.Price < double.Parse(compareTo)).ToListAsync());
                        break;
                    case IsNotEqualTo:
                        products.AddRange(await _context.Products.Where(p => !p.Price.Equals(double.Parse(compareTo))).ToListAsync());
                        break;
                }
            }
            else if (compareSubject == ProductAmountLeft)
            {
                switch (condition)
                {
                    case IsEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Quantity == int.Parse(compareTo)).ToListAsync());
                        break;
                    case IsGreaterThan:
                        products.AddRange(await _context.Products.Where(p => p.Quantity > int.Parse(compareTo)).ToListAsync());
                        break;
                    case IsLessThen:
                        products.AddRange(await _context.Products.Where(p => p.Quantity < int.Parse(compareTo)).ToListAsync());
                        break;
                }
            }
            else if (compareSubject == ProductTitle)
            {
                switch (condition)
                {
                    case IsEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Title == compareTo).ToListAsync());
                        break;
                    case IsNotEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Title != compareTo).ToListAsync());
                        break;
                    case StartsWith:
                        products.AddRange(await _context.Products.Where(p => p.Title.StartsWith(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case EndsWith:
                        products.AddRange(await _context.Products.Where(p => p.Title.EndsWith(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case Contains:
                        products.AddRange(await _context.Products.Where(p => p.Title.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case DoesNotContain:
                        products.AddRange(await _context.Products.Where(p => !p.Title.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                }
            }
            else if (compareSubject == CollectionCompareSubjects.ProductType)
            {
                switch (condition)
                {
                    case IsEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.ProductType.Handle == compareTo).ToListAsync());
                        break;
                    case IsNotEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.ProductType.Handle != compareTo).ToListAsync());
                        break;
                    case StartsWith:
                        products.AddRange(await _context.Products.Where(p => p.ProductType.Handle.StartsWith(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case EndsWith:
                        products.AddRange(await _context.Products.Where(p => p.ProductType.Handle.EndsWith(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case Contains:
                        products.AddRange(await _context.Products.Where(p => p.ProductType.Handle.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case DoesNotContain:
                        products.AddRange(await _context.Products.Where(p => !p.ProductType.Handle.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                }
            }
            else if (compareSubject == ProductVendor)
            {
                switch (condition)
                {
                    case IsEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Vendor == compareTo).ToListAsync());
                        break;
                    case IsNotEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Vendor != compareTo).ToListAsync());
                        break;
                    case StartsWith:
                        products.AddRange(await _context.Products.Where(p => p.Vendor.StartsWith(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case EndsWith:
                        products.AddRange(await _context.Products.Where(p => p.Vendor.EndsWith(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case Contains:
                        products.AddRange(await _context.Products.Where(p => p.Vendor.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                    case DoesNotContain:
                        products.AddRange(await _context.Products.Where(p => !p.Vendor.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase)).ToListAsync());
                        break;
                }
            }
            else if (compareSubject == ProductWeight)
            {
                switch (condition)
                {
                    case IsEqualTo:
                        products.AddRange(await _context.Products.Where(p => p.Weight.Equals(double.Parse(compareTo))).ToListAsync());
                        break;
                    case IsGreaterThan:
                        products.AddRange(await _context.Products.Where(p => p.Weight > double.Parse(compareTo)).ToListAsync());
                        break;
                    case IsLessThen:
                        products.AddRange(await _context.Products.Where(p => p.Weight < double.Parse(compareTo)).ToListAsync());
                        break;
                    case IsNotEqualTo:
                        products.AddRange(await _context.Products.Where(p => !p.Weight.Equals(double.Parse(compareTo))).ToListAsync());
                        break;
                }
            }

            return products;
        }

        /// <summary>
        /// Фильтрация списка продуктов по новым условиям
        /// </summary>
        /// <returns>Список продуктов</returns>
        /// <param name="products">Список продуктов для фильтрации</param>
        /// <param name="compareSubject">Субъект сравнения</param>
        /// <param name="condition">Условие для проверки</param>
        /// <param name="compareTo">Объект сравнения</param>
        private List<Product> ApplyNewConditionsToProductList(List<Product> products, CollectionCompareSubjects compareSubject, CollectionConditions condition, string compareTo)
        {
            List<Product> newProducts = new List<Product>();

            foreach (Product p in products)
            {
                if (compareSubject == ProductTag)
                {
                    switch (condition)
                    {
                        case IsEqualTo:
                            if (p.Tags.Contains(compareTo))
                            {
                                newProducts.Add(p);
                            }
                            break;
                    }
                }
                else if (compareSubject == ProductWeight || compareSubject == ProductPrice)
                {
                    double value;

                    if (compareSubject == ProductWeight)
                    {
                        value = p.Weight;
                    }
                    else
                    {
                        value = p.Price;
                    }

                    switch (condition)
                    {
                        case IsEqualTo:
                            if (value.Equals(double.Parse(compareTo)))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case IsGreaterThan:
                            if (value > double.Parse(compareTo))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case IsLessThen:
                            if (value < double.Parse(compareTo))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case IsNotEqualTo:
                            if (!value.Equals(double.Parse(compareTo)))
                            {
                                newProducts.Add(p);
                            }
                            break;
                    }
                }
                else if (compareSubject == ProductAmountLeft)
                {
                    switch (condition)
                    {
                        case IsEqualTo:
                            if (p.Quantity == int.Parse(compareTo))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case IsGreaterThan:
                            if (p.Quantity > int.Parse(compareTo))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case IsLessThen:
                            if (p.Quantity < int.Parse(compareTo))
                            {
                                newProducts.Add(p);
                            }
                            break;
                    }
                }
                else if (compareSubject == ProductTitle || compareSubject == CollectionCompareSubjects.ProductType || compareSubject == ProductVendor)
                {
                    string value;

                    if (compareSubject == ProductTitle)
                    {
                        value = p.Title;
                    }
                    else if (compareSubject == CollectionCompareSubjects.ProductType)
                    {
                        value = p.ProductType.Handle;
                    }
                    else
                    {
                        value = p.Vendor;
                    }

                    switch (condition)
                    {
                        case IsEqualTo:
                            if (value == compareTo)
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case IsNotEqualTo:
                            if (value != compareTo)
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case StartsWith:
                            if (value.StartsWith(compareTo, StringComparison.CurrentCultureIgnoreCase))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case EndsWith:
                            if (value.EndsWith(compareTo, StringComparison.CurrentCultureIgnoreCase))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case Contains:
                            if (value.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase))
                            {
                                newProducts.Add(p);
                            }
                            break;
                        case DoesNotContain:
                            if (!value.Contains(compareTo, StringComparison.CurrentCultureIgnoreCase))
                            {
                                newProducts.Add(p);
                            }
                            break;
                    }
                }
            }

            return newProducts;
        }
    }
}
