using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VeorCollection.Data;
using VeorCollection.Models;

namespace VeorCollection.Controllers
{
    public class VeorCollectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeorCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Anasayfa
        public IActionResult Index()
        {
            return View();
        }

        // --- GÜNCELLENEN KISIM: PRODUCTS METODU ---
        // Yeni parametre eklendi: categoryId
        public IActionResult Products(int? genderId, int? scentTypeId, int? categoryId)
        {
            // 1. Temel Sorgu: Tüm ilişkili verileri getir
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Gender)
                .Include(p => p.ScentType)
                .AsQueryable();

            // 2. Kategori Filtresi (YENİ)
            if (categoryId.HasValue)
            {
                products = products.Where(x => x.CategoryId == categoryId.Value);
                ViewBag.SeciliCategoryId = categoryId.Value;
            }

            // 3. Cinsiyet Filtresi
            if (genderId.HasValue)
            {
                products = products.Where(x => x.GenderId == genderId.Value);
                ViewBag.SeciliGenderId = genderId.Value;
            }

            // 4. Koku Tipi Filtresi
            if (scentTypeId.HasValue)
            {
                products = products.Where(x => x.ScentTypeId == scentTypeId.Value);
                ViewBag.SeciliScentTypeId = scentTypeId.Value;
            }

            // --- AKILLI FİLTRE MANTIĞI ---
            // Varsayılan olarak koku filtresini göster
            bool showScentFilter = true;

            // Eğer bir kategori seçiliyse (örn: Takı) ve bu kategoride hiç koku özelliği yoksa filtreyi gizle
            if (categoryId.HasValue)
            {
                // Veritabanına soruyoruz: Bu kategori ID'sine sahip ve ScentType'ı dolu olan ürün var mı?
                bool hasScentProducts = _context.Products
                    .Any(p => p.CategoryId == categoryId.Value && p.ScentTypeId != null);

                // Eğer koku tipi olan ürün yoksa filtreyi kapat
                if (!hasScentProducts)
                {
                    showScentFilter = false;
                }
            }

            // View tarafına gerekli bilgileri gönderiyoruz
            ViewBag.ShowScentFilter = showScentFilter;     // Filtre gizlensin mi?
            ViewBag.Categories = _context.Categories.ToList(); // Sidebar için kategoriler
            ViewBag.Genders = _context.Genders.ToList();
            ViewBag.ScentTypes = _context.ScentTypes.ToList();

            return View(products.ToList());
        }
        // ------------------------------------------

        // Ürün Detay Sayfası
        public IActionResult ShopDetail(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("Products");
            }

            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Gender)
                .Include(p => p.ScentType)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return RedirectToAction("Products");
            }

            return View(product);
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        // Seed Data
        public IActionResult SeedData()
        {
            bool veriEklendi = false;

            if (!_context.Genders.Any())
            {
                _context.Genders.AddRange(
                    new Gender { Name = "Erkek" },
                    new Gender { Name = "Kadın" },
                    new Gender { Name = "Unisex" }
                );
                veriEklendi = true;
            }

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
                return Content("Başarılı: Veriler eklendi.");
            }
            else
            {
                return Content("Bilgi: Veriler zaten mevcut.");
            }
        }
    }
}