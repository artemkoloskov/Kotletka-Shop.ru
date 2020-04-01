using System;
namespace KotletkaShop.Models
{
    public class Payment
    {
        // Модель EF
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
        public int OrderID { get; set; }
        public DateTime DatePaid { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }

        public Order Order { get; set; }
        public Customer Customer { get; set; }
    }
}
