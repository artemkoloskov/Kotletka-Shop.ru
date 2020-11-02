namespace KotletkaShop.Models
{
	public class OrderDiscount
	{
		// Модель EF
		public int ID { get; set; }
		public int OrderID { get; set; }
		public int DiscountID { get; set; }

		public Order Order { get; set; }
		public Discount Discount { get; set; }
	}
}
