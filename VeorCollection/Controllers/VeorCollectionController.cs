using Microsoft.AspNetCore.Mvc;
using VeorCollection.Data;
using System.Linq;

namespace VeorCollection.Controllers
{
    public class VeorCollectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Veritabanı bağlantısını içeri alıyoruz
        public VeorCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ÜRÜNLER SAYFASI
        public IActionResult Products()
        {
            // Veritabanındaki tüm ürünleri listeye çevirip View'a gönderiyoruz
            var urunler = _context.Products.ToList();
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