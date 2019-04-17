using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KotletkaShop.Data;
using Microsoft.EntityFrameworkCore;
using static KotletkaShop.Models.CollectionCompareSubjects;
using static KotletkaShop.Models.CollectionConditions;

namespace KotletkaShop.Models
{
    public enum CollectionCompareSubjects
    {
        ProductTitle = 1,
        ProductTag,
        ProductType,
        ProductVendor,
        ProductPrice,
        ProductWeight,
        ProductAmountLeft
    }

    public enum CollectionConditions
    {
        IsEqualTo = 1,
        IsNotEqualTo,
        IsGreaterThan,
        IsLessThen,
        StartsWith,
        EndsWith,
        Contains,
        DoesNotContain
    }

    public enum SortingOrders
    {
        A_Z = 1,
        Z_A,
        HighestPrice,
        LowestPrice,
        Manual
    }

    public class Collection
    {
        public int CollectionID { get; set; }
        public string Handle { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; } = false;
        public CollectionCompareSubjects CompareSubject1 { get; set; }
        public CollectionConditions Condition1 { get; set; }
        public string CompareTo1 { get; set; }
        public CollectionCompareSubjects CompareSubject2 { get; set; }
        public CollectionConditions Condition2 { get; set; }
        public string CompareTo2 { get; set; }
        public CollectionCompareSubjects CompareSubject3 { get; set; }
        public CollectionConditions Condition3 { get; set; }
        public string CompareTo3 { get; set; }
        public bool MatchAllConditions { get; set; } = false;
        public int ImageID { get; set; }
        public SortingOrders SortBy { get; set; } = SortingOrders.A_Z;

        public Image Image { get; set; }

        [NotMapped]
        public List<Product> Products { get; set; }

        /// <summary>
        /// Преобразует условия записанные в объект в читаемую форму
        /// </summary>
        /// <value>Строка с условиями наполнения коллекции в читаемоей форме</value>
        public List<string> ScreenConditions
        {
            get
            {
                List<string> conditions = new List<string>();

                string condition = TranslateCompareSubj(CompareSubject1) + " " + TranslateCondition(Condition1) + " " + CompareTo1;
                conditions.Add(condition);

                condition = TranslateCompareSubj(CompareSubject2) + " " + TranslateCondition(Condition2) + " " + CompareTo2;
                conditions.Add(condition);

                condition = TranslateCompareSubj(CompareSubject3) + " " + TranslateCondition(Condition3) + " " + CompareTo3;
                conditions.Add(condition);

                return conditions;
            }
        }

        /// <summary>
        /// Переводит значениям нумератора типов объектов для сравнения в читаемую форму
        /// </summary>
        /// <returns>Тип объекта для сравнения</returns>
        /// <param name="compareSubject">ип объекта для сравнения в читаемой форме</param>
        private string TranslateCompareSubj(CollectionCompareSubjects compareSubject)
        {
            string translatedString = "";

            switch (compareSubject)
            {
                case CollectionCompareSubjects.ProductType:
                    translatedString = "Тип товара"; // LOCALIZE 
                    return translatedString;
                case CollectionCompareSubjects.ProductTag:
                    translatedString = "Тэг товара"; // LOCALIZE
                    return translatedString;
                case CollectionCompareSubjects.ProductPrice:
                    translatedString = "Стоимость товара"; // LOCALIZE
                    return translatedString;
                case CollectionCompareSubjects.ProductWeight:
                    translatedString = "Вес товара"; // LOCALIZE
                    return translatedString;
                case CollectionCompareSubjects.ProductVendor:
                    translatedString = "Производитель товара"; // LOCALIZE
                    return translatedString;
                case CollectionCompareSubjects.ProductTitle:
                    translatedString = "Название товара"; // LOCALIZE
                    return translatedString;
                case CollectionCompareSubjects.ProductAmountLeft:
                    translatedString = "Количество товара на складе"; // LOCALIZE
                    return translatedString;
            }

            return translatedString;
        }

        /// <summary>
        /// Переводит типы условий в читаемую форму
        /// </summary>
        /// <returns>Тип условия</returns>
        /// <param name="condition">Тип условия в читаемой форме.</param>
        private string TranslateCondition(CollectionConditions condition)
        {
            string translatedString = "";

            switch (condition)
            {
                case CollectionConditions.Contains:
                    translatedString = "содержит"; // LOCALIZE
                    return translatedString;
                case CollectionConditions.IsEqualTo:
                    translatedString = "равно"; // LOCALIZE
                    return translatedString;
                case CollectionConditions.IsNotEqualTo:
                    translatedString = "не равно"; // LOCALIZE
                    return translatedString;
                case CollectionConditions.IsGreaterThan:
                    translatedString = "больше, чем"; // LOCALIZE
                    return translatedString;
                case CollectionConditions.IsLessThen:
                    translatedString = "меньше, чем"; // LOCALIZE
                    return translatedString;
                case CollectionConditions.StartsWith:
                    translatedString = "начинается с"; // LOCALIZE
                    return translatedString;
                case CollectionConditions.EndsWith:
                    translatedString = "заканчивается на"; // LOCALIZE
                    return translatedString;
                case CollectionConditions.DoesNotContain:
                    translatedString = "не содержит"; // LOCALIZE
                    return translatedString;
            }

            return translatedString;
        }

        /// <summary>
        /// Поиск продуктов для коллекции
        /// </summary>
        /// <returns>Список продуктов</returns>
        /// <param name="_context">Контекст БД.</param>
        public async Task<List<Product>> GetCollectionProducts(StoreContext _context)
        {
            List<Product> products = new List<Product>();

            if (MatchAllConditions)
            {
                if (CompareSubject1 > 0 && Condition1 > 0 && CompareTo1 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(_context, CompareSubject1, Condition1, CompareTo1));
                }
                if (CompareSubject2 > 0 && Condition2 > 0 && CompareTo2 != "")
                {
                    products = ApplyNewConditionsToProductList(products, CompareSubject2, Condition2, CompareTo2);
                }
                if (CompareSubject3 > 0 && Condition3 > 0 && CompareTo3 != "")
                {
                    products = ApplyNewConditionsToProductList(products, CompareSubject3, Condition3, CompareTo3);
                }

            }
            else
            {
                if (CompareSubject1 > 0 && Condition1 > 0 && CompareTo1 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(_context, CompareSubject1, Condition1, CompareTo1));
                }
                if (CompareSubject2 > 0 && Condition2 > 0 && CompareTo2 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(_context, CompareSubject2, Condition2, CompareTo2));
                }
                if (CompareSubject3 > 0 && Condition3 > 0 && CompareTo3 != "")
                {
                    products.AddRange(await GetCollectionProductsByConditions(_context, CompareSubject3, Condition3, CompareTo3));
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
        private async Task<List<Product>> GetCollectionProductsByConditions(StoreContext _context, CollectionCompareSubjects compareSubject, CollectionConditions condition, string compareTo)
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
