using Microsoft.EntityFrameworkCore;
using VeorCollection.Models; // Bu satır 'User' sınıfını tanıması için ŞART

namespace VeorCollection.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }      // Hata veren kısım burasıydı
        public DbSet<CartItem> CartItems { get; set; }
    }
}