// ApplicationDbContext.cs
using IdentityAppCourse2022.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityAppCourse2022.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Order> Orders { get; set; } // Добавлено
        public DbSet<OrderItem> OrderItems { get; set; } // Добавлено

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .Ignore(p => p.RoleId);

            builder.Entity<AppUser>()
                .Ignore(p => p.Role);

            builder.Entity<AppUser>()
                .Ignore(p => p.RoleList);

            builder.Entity<Product>()
                .Ignore(p => p.CategoryList);

            builder.Entity<Category>()
                .HasMany(p => p.Products);

            builder.Entity<AppUser>()
                .HasMany(p => p.products);

            builder.Entity<Product>()
                .HasOne(c => c.category);

            builder.Entity<Product>()
                .HasOne(c => c.provider);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
                    .HasMany(o => o.OrderItems);

            // убираем каскадное удаление в OrderItem
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order);
      }
    }
}
