using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
        public string Image { get; set; }
        public string ImageAltText { get; set; }
        public SortingOrders SortBy { get; set; } = SortingOrders.A_Z;

        [NotMapped]
        public List<Product> Products { get; set; }

        public List<string> ScreenConditions
        {
            get 
            {
                List<string> conditions = new List<string> ();

                string condition = TranslateCompareSubj(CompareSubject1) + " " + TranslateCondition(Condition1) + " " + CompareTo1;
                conditions.Add(condition);

                condition = TranslateCompareSubj(CompareSubject2) + " " + TranslateCondition(Condition2) + " " + CompareTo2;
                conditions.Add(condition);

                condition = TranslateCompareSubj(CompareSubject3) + " " + TranslateCondition(Condition3) + " " + CompareTo3;
                conditions.Add(condition);

                return conditions;
            }
        }

        private string TranslateCompareSubj (CollectionCompareSubjects compareSubject)
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

        private string TranslateCondition (CollectionConditions condition)
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
    }
}
