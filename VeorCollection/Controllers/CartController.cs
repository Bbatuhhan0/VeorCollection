using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using VeorCollection.Data;
using VeorCollection.Models;

namespace VeorCollection.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar sepeti kullanabilir
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // SEPETİM SAYFASI (Listeleme)
        public IActionResult Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null) return RedirectToAction("Login", "Account");

            // Kullanıcının sepetindeki ürünleri getir (Ürün detaylarıyla beraber)
            var cartItems = _context.CartItems
                                    .Include(c => c.Product)
                                    .Where(c => c.UserId == user.Id)
                                    .ToList();

            // Toplam tutarı hesapla ve View'a gönder
            ViewBag.Total = cartItems.Sum(c => c.Product.Price * c.Quantity);

            return View(cartItems);
        }

        // SEPETE EKLEME İŞLEMİ
        public IActionResult AddToCart(int productId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null) return RedirectToAction("Login", "Account");

            // Bu ürün zaten sepette var mı?
            var existingItem = _context.CartItems.FirstOrDefault(c => c.UserId == user.Id && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++; // Varsa adedini artır
            }
            else
            {
                // Yoksa yeni ekle
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Quantity = 1
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return RedirectToAction("Index"); // Sepete yönlendir
        }

        // SEPETTEN SİLME
        public IActionResult Remove(int id)
        {
            var item = _context.CartItems.Find(id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}