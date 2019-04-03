using System;
using System.Collections.Generic;
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

    public enum DiscountApplyableObjectTypes
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
        public DiscountApplyableObjectTypes AppliesTo { get; set; }
        public string ApplyableObjects { get; set; }
        public DiscountMinimumRequirementTypes MinimumRequirement { get; set; }
        public double MinimumRequirementValue { get; set; }
        public DiscountEligibleObjectTypes CustomerEligibility { get; set; }
        public string EligibleObjects { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IEnumerable<Order> Orders { get; set; }

        public List<int> ApplyableObjectsIDs
        {
            get
            {
                List<int> ids = new List<int> ();

                ids = ApplyableObjects.Split(',').Select(Int32.Parse).ToList();

                return ids;
            }
        }

        public List<int> EligibleObjectsIDs
        {
            get
            {
                List<int> ids = new List<int> ();

                ids = EligibleObjects.Split(',').Select(Int32.Parse).ToList();

                return ids;
            }
        }
    }
}
