using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeorCollection.Data;
using VeorCollection.Models;
using System.Linq;

namespace VeorCollection.Controllers
{
    // Bu Controller sadece site sahibinin (senin) veri girişi yapması içindir.
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Paneli Giriş Sayfası
        public IActionResult Index()
        {
            return View();
        }

        // --- KATEGORİ YÖNETİMİ ---

        // Kategorileri Listele
        public IActionResult Categories()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // Yeni Kategori Ekleme Sayfasını Aç
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        // Kategori Formunu Kaydet
        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Categories"); // Listeye geri dön
            }
            return View(category);
        }

        // Kategori Sil
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("Categories");
        }

        // --- ÜRÜN YÖNETİMİ ---

        // Yeni Ürün Ekleme Sayfasını Aç
        [HttpGet]
        public IActionResult CreateProduct()
        {
            // Ürün eklerken hangi kategoriye ait olduğunu seçmemiz lazım.
            // Bu yüzden veritabanındaki kategorileri çekip "Dropdown" (açılır kutu) için hazırlıyoruz.
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // Ürün Formunu Kaydet
        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Categories"); // Şimdilik kategori listesine dönsün
            }

            // Hata varsa (örneğin fiyat boşsa) kutuyu tekrar doldurması için kategorileri yine gönderiyoruz
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(product);
        }
    }
}