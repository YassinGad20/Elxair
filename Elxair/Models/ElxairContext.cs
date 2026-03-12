using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class ElxairContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=Elxair;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Perfume> Perfumes { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
