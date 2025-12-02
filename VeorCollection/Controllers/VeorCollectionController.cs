using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include metodunun çalışması için bu ŞART
using System.Linq;
using VeorCollection.Data;
using VeorCollection.Models;

namespace VeorCollection.Controllers
{
    public class VeorCollectionController : Controller
    {
        // Veritabanı bağlantısı için Context nesnesi
        private readonly ApplicationDbContext _context;

        // Constructor (Yapıcı Metot)
        public VeorCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Anasayfa
        public IActionResult Index()
        {
            return View();
        }

        // Ürünler Sayfası ve Filtreleme Mantığı (GÜNCELLENDİ)
        public IActionResult Products(int? genderId, int? scentTypeId)
        {
            // 1. Ürünleri; Kategorisi, Cinsiyeti ve Koku Tipi ile beraber çekiyoruz
            var products = _context.Products
                .Include(p => p.Category)   // Kategori bilgisini dahil et
                .Include(p => p.Gender)     // Cinsiyet bilgisini dahil et
                .Include(p => p.ScentType)  // Koku tipi bilgisini dahil et
                .AsQueryable();

            // 2. Cinsiyet Filtresi (ID'ye göre)
            if (genderId.HasValue)
            {
                products = products.Where(x => x.GenderId == genderId.Value);
                ViewBag.SeciliGenderId = genderId.Value; // Seçili filtreyi View'da işaretlemek için
            }

            // 3. Koku Tipi Filtresi (ID'ye göre)
            if (scentTypeId.HasValue)
            {
                products = products.Where(x => x.ScentTypeId == scentTypeId.Value);
                ViewBag.SeciliScentTypeId = scentTypeId.Value; // Seçili filtreyi View'da işaretlemek için
            }

            // 4. Sidebar'daki menüleri doldurmak için veritabanından listeleri çekip View'a gönderiyoruz
            ViewBag.Genders = _context.Genders.ToList();
            ViewBag.ScentTypes = _context.ScentTypes.ToList();

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

        // --- VERİ EKLEME METODU (SEED DATA) ---
        // Bu metodu bir kez çalıştırdıktan sonra silebilirsiniz.
        public IActionResult SeedData()
        {
            bool veriEklendi = false;

            // Eğer Cinsiyet tablosu boşsa ekle
            if (!_context.Genders.Any())
            {
                _context.Genders.AddRange(
                    new Gender { Name = "Erkek" },
                    new Gender { Name = "Kadın" },
                    new Gender { Name = "Unisex" }
                );
                veriEklendi = true;
            }

            // Eğer Koku Tipi tablosu boşsa ekle
            if (!_context.ScentTypes.Any())
            {
                _context.ScentTypes.AddRange(
                    new ScentType { Name = "Odunsu" },
                    new ScentType { Name = "Çiçeksi" },
                    new ScentType { Name = "Meyveli" },
                    new ScentType { Name = "Baharatlı" },
                    new ScentType { Name = "Ferah" },
                    new ScentType { Name = "Şekerli" }
                );
                veriEklendi = true;
            }

            if (veriEklendi)
            {
                _context.SaveChanges();
                return Content("Başarılı: Cinsiyet ve Koku Tipi verileri veritabanına eklendi!");
            }
            else
            {
                return Content("Bilgi: Veritabanında veriler zaten mevcut, ekleme yapılmadı.");
            }
        }
        // -------------------------------------
    }
}