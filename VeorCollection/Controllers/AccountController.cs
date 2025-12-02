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
        // 1. MÜŞTERİ (KULLANICI) GİRİŞİ
        // Normal "Giriş Yap" sayfası burayı kullanır
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Uye"))
            {
                return RedirectToAction("Index", "VeorCollection");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Sadece veritabanındaki kullanıcıları arar
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Uye") // Rol: UYE
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "VeorCollection");
            }

            ViewBag.Error = "E-posta veya şifre hatalı!";
            return View();
        }

        // ==========================================
        // 2. YÖNETİCİ GİRİŞİ (GİZLİ KAPI)
        // Sadece "/Account/AdminLogin" adresine gidenler görebilir
        // ==========================================
        [HttpGet]
        public IActionResult AdminLogin()
        {
            // Eğer zaten admin ise direkt panele at
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(string username, string password)
        {
            // Sadece bu şifreyi bilenler Admin olabilir
            if (username == "admin" && password == "12345")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Yönetici"),
                    new Claim(ClaimTypes.Role, "Admin") // Rol: ADMIN
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Admin olarak giriş yap
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Admin Paneline yönlendir
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Yönetici bilgileri hatalı!";
            return View();
        }

        // ==========================================
        // 3. KAYIT OL & ÇIKIŞ
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "VeorCollection");
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

            user.Role = "Uye";
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "VeorCollection");
        }
    }
}