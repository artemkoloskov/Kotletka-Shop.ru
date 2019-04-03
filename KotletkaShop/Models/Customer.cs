using System;
using System.Collections.Generic;

namespace KotletkaShop.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Apartment { get; set; }
        public int ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public bool AcceptsMarketing { get; set; } = false;
        public DateTime RegisterDate { get; set; }
        public string Image { get; set; }
        public string Tags { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Payment> Payments { get; set; }

        public string ScreenName
        {
            get
            {
                if (LastName != null && FirstName != null)
                {
                    return LastName + " " + FirstName;
                }

                return Email;
            }
        }

        public string ScreenAddressShort => City + ", " + Province + ", " + Country;

        public double TotalSpent
        {
            get
            {
                double totalSpent = 0;

                foreach (Payment p in Payments)
                {
                    totalSpent += p.Amount;
                }

                return totalSpent;
            }
        }
    }
}
