using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace VeorCollection.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            // Eğer zaten giriş yapmışsa direkt panele at (Tekrar tekrar sormasın)
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // BURAYI KENDİ ŞİFRENLE GÜNCELLEMEYİ UNUTMA
            if (username == "admin" && password == "12345")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // --- İŞTE KRİTİK AYAR BURASI ---
                var authProperties = new AuthenticationProperties
                {
                    // IsPersistent = false: Tarayıcı kapanınca oturumu sil (RAM'de tut)
                    // IsPersistent = true: Bilgisayarı kapatsan bile hatırla (Harddisk'e yaz)
                    IsPersistent = false,

                    // İsteğe bağlı: Hareketsiz kalırsa 20 dk sonra otomatik at
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        // Çıkış Yapma Butonu İçin
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}