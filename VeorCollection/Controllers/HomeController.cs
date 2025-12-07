using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Bu kütüphaneyi eklemeyi unutma
using System.Diagnostics;
using System.Linq;
using VeorCollection.Data;
using VeorCollection.Models;

namespace VeorCollection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Veritabaný baðlantýsý

        // Constructor'a context'i ekliyoruz
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Veritabanýndan en son eklenen 8 ürünü çek (Id'si en büyük olanlar en yenidir)
            var recentProducts = _context.Products
                                         .OrderByDescending(p => p.Id)
                                         .Take(8)
                                         .ToList();

            // Ürünleri View'a (sayfaya) gönderiyoruz
            return View(recentProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}