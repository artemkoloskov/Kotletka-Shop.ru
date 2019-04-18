using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KotletkaShop.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Handle { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Vendor { get; set; }
        public int ProductTypeID { get; set; }
        public string Tags { get; set; }
        public bool Published { get; set; } = false;
        public string Option1Name { get; set; }
        public string Option1Value { get; set; }
        public string Option2Name { get; set; }
        public string Option2Value { get; set; }
        public string Option3Name { get; set; }
        public string Option3Value { get; set; }
        public int Weight { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime VisibleFrom { get; set; }
        public DateTime VisibleUntil { get; set; }

        public ProductType ProductType { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }

        [NotMapped]
        public List<Collection> Collections { get; set; } = new List<Collection>();
    }
}
