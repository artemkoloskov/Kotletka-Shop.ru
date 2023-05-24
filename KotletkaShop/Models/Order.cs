using System;
using System.Collections.Generic;
using static KotletkaShop.Models.DiscountApplicableObjectTypes;
using static KotletkaShop.Models.DiscountEligibleObjectTypes;
using static KotletkaShop.Models.DiscountMinimumRequirementTypes;
using static KotletkaShop.Models.DiscountTypes;

namespace KotletkaShop.Models
{
    public class Order
    {
        // Модель EF
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Paid { get; set; } = false;
        public DateTime DatePaid { get; set; }
        public bool Fulfilled { get; set; } = false;
        public DateTime DateFulfilled { get; set; }
        public bool Shipped { get; set; } = false;
        public DateTime DateShipped { get; set; }
        public bool Canceled { get; set; } = false;
        public DateTime DateCanceled { get; set; }
        public int ShippingID { get; set; }
        public double ShippingCost { get; set; }
        public string Note { get; set; }
        public bool Draft { get; set; } = true;

        public Customer Customer { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; }
        public ICollection<OrderDiscount> OrderDiscounts { get; set; }

        /// <summary>
        /// Подсчитывает количество штук товаров в заказе
        /// </summary>
        public int AmountOfProducts
        {
            get
            {
                int amount = 0;

                foreach (OrderProduct p in OrderProducts)
                {
                    amount += p.Quantity;
                }

                return amount;
            }
        }

        /// <summary>
        /// Сумма за заказ, без учета скидки и доставки
        /// </summary>
        public double Cost
        {
            get
            {
                double cost = 0;

                foreach (OrderProduct p in OrderProducts)
                {
                    cost += p.Quantity * p.Product.Price;
                }

                return cost;
            }
        }

        /// <summary>
        /// Подсчитывает сумму скидки
        /// </summary>
        public double DiscountAmount
        {
            get
            {
                double discountAmount = 0;
                bool shippingIsAlreadyFree = false;

                foreach (OrderDiscount d in OrderDiscounts)
                {
                    if (d.Discount.IsActive)
                    {
                        if (d.Discount.CustomerEligibility == Everyone ||
                            (d.Discount.CustomerEligibility == SpecificCustomers && d.Discount.EligibleObjectsIDs.Contains(CustomerID))) // TODO добавить проверку на группы и сами группы покупателей
                        {
                            if ((d.Discount.MinimumRequirement == MinimumAmount && Cost >= d.Discount.MinimumRequirementValue) ||
                                (d.Discount.MinimumRequirement == MinimumQuantity && AmountOfProducts >= d.Discount.MinimumRequirementValue) ||
                                d.Discount.MinimumRequirement == None)
                            {
                                if (d.Discount.AppliesTo == EntireOrder)
                                {
                                    switch (d.Discount.Type)
                                    {
                                        case FixedAmount:
                                            discountAmount += d.Discount.Value;
                                            break;
                                        case FreeShiping:
                                            if (!shippingIsAlreadyFree)
                                            {
                                                discountAmount += ShippingCost;
                                                shippingIsAlreadyFree = true;
                                            }

                                            break;
                                        case Percentage:
                                            discountAmount += Cost * d.Discount.Value;
                                            break;
                                        case BuyXGetY:
                                            break; // TODO добавить этот случай
                                        default:
                                            break;
                                    }
                                }
                                else if (d.Discount.AppliesTo == SpecificProducts)
                                {
                                    foreach (OrderProduct p in OrderProducts)
                                    {
                                        if (d.Discount.ApplyableObjectsIDs.Contains(p.ProductID))
                                        {
                                            switch (d.Discount.Type)
                                            {
                                                case FixedAmount:
                                                    discountAmount += d.Discount.Value;
                                                    break;
                                                case FreeShiping:
                                                    if (!shippingIsAlreadyFree)
                                                    {
                                                        discountAmount += ShippingCost;
                                                        shippingIsAlreadyFree = true;
                                                    }

                                                    break;
                                                case Percentage:
                                                    discountAmount += p.Product.Price * p.Quantity * d.Discount.Value;
                                                    break;
                                                case BuyXGetY: // TODO добавить этот случай
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else if (d.Discount.AppliesTo == SpecificCollections) // TODO добавить этот случай
                                {

                                }
                            }
                        }
                    }
                }

                return discountAmount;
            }
        }

        /// <summary>
        /// Подсчитывает полную стоимость за заказ, с учетом доставки и скидок
        /// </summary>
        public double TotalCost => Cost + ShippingCost - DiscountAmount;

        /// <summary>
        /// Стоимость за зака с учетом скидки, без учета доставки
        /// </summary>
        public double DiscountedCost => Cost - DiscountAmount;

        /// <summary>
        /// Сумма оплаченая клиентом за заказ
        /// </summary>
        public double TotalPaid
        {
            get
            {
                double totalPaid = 0;

                foreach (Payment p in Payments)
                {
                    totalPaid += p.Amount;
                }

                return totalPaid;
            }
        }
    }
}
