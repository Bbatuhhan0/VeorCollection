using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VeorCollection.Data;

namespace VeorCollection.Controllers
{
    public class VeorCollectionController : Controller
    {
        // Veritabanı bağlantısı için Context nesnesi
        private readonly ApplicationDbContext _context;

        // Constructor (Yapıcı Metot) - Veritabanı bağlantısını başlatır
        public VeorCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Anasayfa (VeorCollection/Index)
        public IActionResult Index()
        {
            return View();
        }

        // Ürünler Sayfası ve Filtreleme Mantığı
        public IActionResult Products(string cinsiyet, string kokuTipi)
        {
            // 1. Veritabanındaki tüm ürünleri sorgulanabilir olarak al
            var products = _context.Products.AsQueryable();

            // 2. Eğer URL'den "cinsiyet" parametresi geldiyse (Örn: ?cinsiyet=Erkek) filtrele
            if (!string.IsNullOrEmpty(cinsiyet))
            {
                products = products.Where(x => x.Cinsiyet == cinsiyet);

                // Seçili filtreyi View tarafında (HTML'de) kullanmak için ViewBag'e atıyoruz
                ViewBag.SeciliCinsiyet = cinsiyet;
            }

            // 3. Eğer URL'den "kokuTipi" parametresi geldiyse (Örn: ?kokuTipi=Odunsu) filtrele
            if (!string.IsNullOrEmpty(kokuTipi))
            {
                products = products.Where(x => x.KokuTipi == kokuTipi);
                ViewBag.SeciliKoku = kokuTipi;
            }

            // 4. Filtrelenmiş listeyi sayfaya gönder
            return View(products.ToList());
        }

        // Blog Sayfası
        public IActionResult Blog()
        {
            return View();
        }

        // Hakkımızda Sayfası
        public IActionResult About()
        {
            return View();
        }
    }
}