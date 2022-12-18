namespace KotletkaShop.Models
{
    public class ProductImage
    {
        // Модель EF
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int ImageID { get; set; }
        public bool IsDefaultImage { get; set; } = false;

        public Product Product { get; set; }
        public Image Image { get; set; }
    }
}
