using System;
using System.Collections.Generic;

namespace KotletkaShop.Models
{
    public class ProductType
    {
        // Модель EF
        public int ProductTypeID { get; set; }
        public string Handle { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
