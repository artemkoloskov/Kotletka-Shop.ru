using System;
namespace KotletkaShop.Models
{
    public class Image
    {
        public int ImageID { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string AltText { get; set; }
    }
}
