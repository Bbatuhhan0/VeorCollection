using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using VeorCollection.Data;
using VeorCollection.Models;

namespace VeorCollection.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. MÜŞTERİ KAPISI (Veritabanından)
        // ==========================================
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // 1. Veritabanında kullanıcıyı ara
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            // 2. Kullanıcı bulunduysa giriş işlemini yap
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Uye") // Rolü: ÜYE
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Çerezi oluştur ve giriş yap
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Başarılıysa Anasayfaya gönder
                return RedirectToAction("Index", "VeorCollection");
            }

            // 3. Kullanıcı bulunamadıysa hata mesajı göster ve sayfayı tekrar yükle
            ViewBag.Error = "E-posta veya şifre hatalı!";
            return View();
        }

        // ==========================================
        // 2. ADMIN KAPISI (Koddan - 12345)
        // ==========================================
        [HttpGet]
        public IActionResult AdminLogin()
        {
            // Eğer zaten admin içerideyse panele at
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(string username, string password)
        {
            // Admin şifresini buradan kontrol ediyoruz (Admin klasörünle alakası yok, bu sadece giriş izni)
            if (username == "admin" && password == "12345")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Yönetici"),
                    new Claim(ClaimTypes.Role, "Admin") // Rolü: ADMIN
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // İŞTE BURASI: Senin mevcut Admin klasörüne yönlendiriyor
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Yönetici bilgileri hatalı!";
            return View();
        }

        // --- KAYIT OL (Sadece Müşteriler İçin) ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Bu e-posta zaten kayıtlı!";
                return View(user);
            }

            user.Role = "Uye"; // Herkes üye olarak başlar
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Eski: return RedirectToAction("Login");
            // Yeni: Anasayfaya yönlendir
            return RedirectToAction("Index", "VeorCollection");
        }
    }
}