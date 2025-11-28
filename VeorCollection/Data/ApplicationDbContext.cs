using Microsoft.EntityFrameworkCore;
using VeorCollection.Models;

namespace VeorCollection.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        // --- BU KISMI EKLE ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product tablosundaki Price alanı için:
            // Toplam 18 basamak, virgülden sonra 2 basamak olsun (Örn: 12345.67)
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
        // ---------------------
    }
}