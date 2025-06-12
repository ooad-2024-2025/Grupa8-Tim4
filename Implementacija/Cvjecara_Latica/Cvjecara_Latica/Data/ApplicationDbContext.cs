using Cvjecara_Latica.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cvjecara_Latica.Data
{
    //public class ApplicationDbContext : IdentityDbContext staro
    public class ApplicationDbContext : IdentityDbContext<Person>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Report> Reports { get; set; }
     
        public DbSet<ProductOrder> ProductOrders { get; set; }

        public DbSet<ProductSale> ProductSales { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Discount>().ToTable("Discounts");
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Cart>().ToTable("Carts");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Report>().ToTable("Reports");
            modelBuilder.Entity<ProductOrder>().ToTable("ProductOrders");
            modelBuilder.Entity<ProductSale>().ToTable("ProductSales");

            base.OnModelCreating(modelBuilder);
        }

    }
}
