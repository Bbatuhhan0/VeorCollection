using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Bu kütüphane "Include" için şart
using VeorCollection.Data;
using System.Linq;

namespace VeorCollection.Controllers
{
    public class VeorCollectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeorCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Anasayfada belki son 8 ürünü göstermek istersin diye limitleme ekledim
            // Include(p => p.Category): Ürünü çekerken Kategorisini de yanına al demek.
            var urunler = _context.Products
                                  .Include(p => p.Category)
                                  .Take(8)
                                  .ToList();
            return View(urunler);
        }

        public IActionResult Products()
        {
            // Tüm ürünleri kategorileriyle beraber listeye çevirip View'a gönderiyoruz
            var urunler = _context.Products
                                  .Include(p => p.Category)
                                  .ToList();
            return View(urunler);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}