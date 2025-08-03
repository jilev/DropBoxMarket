using DropBoxMarket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DropBoxMarket.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Home" },
                new Category { Id = 3, Name = "Fashion" }
            );

            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Smartphone",
                    Description = "Latest model with advanced features",
                    Price = 699.99m,
                    CategoryId = 1,
                    ImageUrl = "/images/smartphone.jpg"
                },
                new Product
                {
                    Id = 2,
                    Title = "Vacuum Cleaner",
                    Description = "High power vacuum cleaner",
                    Price = 199.99m,
                    CategoryId = 2,
                    ImageUrl = "/images/vacuum.jpg"
                },
                new Product
                {
                    Id = 3,
                    Title = "T-Shirt",
                    Description = "Cotton T-Shirt for everyday use",
                    Price = 19.99m,
                    CategoryId = 3,
                    ImageUrl = "/images/tshirt.jpg"
                }
            );
        }
    }
}
