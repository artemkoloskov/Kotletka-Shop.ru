using KotletkaShop.Models;
using Microsoft.EntityFrameworkCore;

namespace KotletkaShop.Data
{
    public class StoreContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDiscount> OrderDiscounts { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {

        }
    }
}
