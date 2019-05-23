using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KotletkaShop.Models
{
    public enum DiscountTypes
    {
        Percentage = 1,
        FixedAmount,
        FreeShiping,
        BuyXGetY
    }

    public enum DiscountApplicableObjectTypes
    {
        EntireOrder = 1,
        SpecificProducts,
        SpecificCollections
    }

    public enum DiscountEligibleObjectTypes
    {
        Everyone = 1,
        SpecificCustomers,
        SpecificGroupsOfCustomers
    }

    public enum DiscountMinimumRequirementTypes
    {
        None = 1,
        MinimumAmount,
        MinimumQuantity,
    }

    public class Discount
    {
        public int DiscountID { get; set; }
        public string Handle { get; set; }
        public DiscountTypes Type { get; set; }
        public double Value { get; set; }
        public DiscountApplicableObjectTypes AppliesTo { get; set; }
        public string ApplicableObjects { get; set; }
        public DiscountMinimumRequirementTypes MinimumRequirement { get; set; }
        public double MinimumRequirementValue { get; set; }
        public DiscountEligibleObjectTypes CustomerEligibility { get; set; }
        public string EligibleObjects { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.ParseExact("2100-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        public bool IsActive { get; set; } = false;
        public int MaxTimesUsed { get; set; } = 0;
        public bool OneUsePerCustomer { get; set; } = false;
        public int TimesUsed { get; set; } = 0;

        [NotMapped]
        public List<Product> ApplicableProducts { get; set; }
        [NotMapped]
        public List<Collection> ApplicableCollections { get; set; }
        [NotMapped]
        public List<Customer> EligibleCustomers { get; set; }

        public List<int> ApplyableObjectsIDs
        {
            get
            {
                List<int> ids = new List<int> ();

                if (ApplicableObjects != null)
                {
                    ids = ApplicableObjects.Split(',').Select(Int32.Parse).ToList();
                }

                return ids;
            }
        }

        public List<int> EligibleObjectsIDs
        {
            get
            {
                List<int> ids = new List<int> ();

                if (EligibleObjects != null)
                {
                    ids = EligibleObjects.Split(',').Select(Int32.Parse).ToList();
                }

                return ids;
            }
        }

        public double ScreenValue ()
        {
            if (Type == DiscountTypes.Percentage)
            {
                return Value * 100;
            }

            return Value;
        }

        public override string ToString()
        {
            string text = Value + "";

            switch (Type)
            {
                case DiscountTypes.Percentage:
                    text = (Value * 100) + "% ";
                    break;
                case DiscountTypes.FixedAmount:
                    text += " руб. ";  //LOCALIZE
                    break;
                case DiscountTypes.FreeShiping:
                    text = "Доставка бесплатно, "; //LOCALIZE
                    break;
                case DiscountTypes.BuyXGetY:
                    text = "Купи X получи Y, "; // Localize
                    break;
            }

            switch (AppliesTo)
            {
                case DiscountApplicableObjectTypes.EntireOrder:
                    text += "на весь заказ. "; //Localize
                    break;
                case DiscountApplicableObjectTypes.SpecificProducts:
                    text += "на " + ApplyableObjectsIDs.Count() + " товаров. "; //Localize
                    break;
                case DiscountApplicableObjectTypes.SpecificCollections:
                    text += "на " + ApplyableObjectsIDs.Count() + " коллекций. "; //Localize
                    break;
            }

            switch (MinimumRequirement)
            {
                case DiscountMinimumRequirementTypes.None:
                    break;
                case DiscountMinimumRequirementTypes.MinimumAmount:
                    text += "Действует при заказе на сумму от " + MinimumRequirementValue + " руб. "; //Localize
                    break;
                case DiscountMinimumRequirementTypes.MinimumQuantity:
                    text += "Действует при количестве товаров в заказе от " + MinimumRequirementValue + " шт. "; //Localize
                    break;
            }

            switch (CustomerEligibility)
            {
                case DiscountEligibleObjectTypes.Everyone:
                    text += "Действует для всех. "; //Localize
                    break;
                case DiscountEligibleObjectTypes.SpecificCustomers:
                    text += "Действует для " + EligibleObjectsIDs.Count + " пользователей."; //Localize
                    break;
                case DiscountEligibleObjectTypes.SpecificGroupsOfCustomers:
                    text += "Действует для " + EligibleObjectsIDs.Count + " групп пользователей."; //Localize
                    break;
            }

            return text;
        }

        public string TypeToString()
        {
            switch (Type)
            {
                case DiscountTypes.Percentage:
                    return "Процент"; //Localize
                case DiscountTypes.FixedAmount:
                    return "Фиксированная сумма"; //Localize
                case DiscountTypes.FreeShiping:
                    return "Бесплатная доставка"; //Localize
                case DiscountTypes.BuyXGetY:
                    return "Купи X получи Y"; //Localize
            }

            return "";
        }

        public string AppliesToToString()
        {
            switch (AppliesTo)
            {
                case DiscountApplicableObjectTypes.EntireOrder:
                    return "Весь заказ"; //Localize
                case DiscountApplicableObjectTypes.SpecificProducts:
                    return "Определенные товары"; //Localize
                case DiscountApplicableObjectTypes.SpecificCollections:
                    return "Определенные коллекции"; //Localize
            }

            return "";
        }

        public string MinimumRequirementToString()
        {
            switch (MinimumRequirement)
            {
                case DiscountMinimumRequirementTypes.None:
                    return "нет"; //Localize
                case DiscountMinimumRequirementTypes.MinimumAmount:
                    return "Сумма заказа"; //Localize
                case DiscountMinimumRequirementTypes.MinimumQuantity:
                    return "Количество товаров в заказе"; //Localize
            }

            return "";
        }

        public string CustomerEligibilityToString()
        {
            switch (CustomerEligibility)
            {
                case DiscountEligibleObjectTypes.Everyone:
                    return "Для всех"; //Localize
                case DiscountEligibleObjectTypes.SpecificCustomers:
                    return "Для определенных пользователей"; //Localize
                case DiscountEligibleObjectTypes.SpecificGroupsOfCustomers:
                    return "Для определенных групп пользователей"; //Localize
            }

            return "";
        }
    }
}
