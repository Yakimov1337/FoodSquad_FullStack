using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FoodSquad_API.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<OrderMenuItem> OrderMenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Role)
                      .HasConversion(new EnumToStringConverter<UserRole>())
                      .HasColumnType("nvarchar(50)");
            });

            // Users -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            // Users -> MenuItems
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.User)
                .WithMany(u => u.MenuItems)
                .HasForeignKey(mi => mi.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            // Users -> Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            // Order -> MenuItems many-to-many relationship
            modelBuilder.Entity<OrderMenuItem>()
                .HasOne(om => om.Order)
                .WithMany(o => o.MenuItemsWithQuantity)
                .HasForeignKey(om => om.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade deletes here

            modelBuilder.Entity<OrderMenuItem>()
                .HasOne(om => om.MenuItem)
                .WithMany()
                .HasForeignKey(om => om.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade deletes here
        }
    }
}
