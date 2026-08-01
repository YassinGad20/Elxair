using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

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
        public ElxairContext(DbContextOptions<ElxairContext> options)
       : base(options)
        {

        }
        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Perfume> Perfumes { get; set; }

        public DbSet<PerfumeSize> PerfumeSizes { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Promotion> Promotions { get; set; }

        public DbSet<PromotionPerfumeSize> PromotionPerfumeSizes { get; set; }

        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-One: Order <-> Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment -> User (NoAction لكسر الـ cycle)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Order -> User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PromotionPerfumeSize>()
                .HasOne(x => x.Promotion)
                .WithMany(x => x.PromotionPerfumeSizes)
                .HasForeignKey(x => x.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PromotionPerfumeSize>()
                .HasOne(x => x.PerfumeSize)
                .WithMany(x => x.PromotionPerfumeSizes)
                .HasForeignKey(x => x.PerfumeSizeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PromotionPerfumeSize>()
                .HasIndex(x => new { x.PromotionId, x.PerfumeSizeId })
                .IsUnique();
        }


    }
}
