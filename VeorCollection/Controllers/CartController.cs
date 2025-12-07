using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using VeorCollection.Data;
using VeorCollection.Models;

namespace VeorCollection.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- SEPETİM SAYFASI ---
        public IActionResult Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null) return RedirectToAction("Login", "Account");

            // Kullanıcının sepetindeki ürünleri getir
            var cartItems = _context.CartItems
                                    .Include(c => c.Product)
                                    .Where(c => c.UserId == user.Id)
                                    .ToList();

            // ViewBag ile toplam tutarı gönderiyoruz
            ViewBag.Total = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Quantity);

            // View'a List<CartItem> modelini gönderiyoruz
            return View(cartItems);
        }

        // --- SEPETE EKLE / ARTIR ---
        [HttpPost] // Güvenlik için POST tercih edilir
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null) return RedirectToAction("Login", "Account");

            var existingItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == user.Id && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            // Eğer işlem ShopDetail sayfasından geldiyse oraya, değilse Sepet'e dön
            // Basitlik adına şimdilik sepete yönlendiriyoruz:
            return RedirectToAction("Index");
        }

        // --- ADET DÜŞÜR ---
        [HttpPost]
        public IActionResult Decrease(int id) // id burada ProductId olarak gelecek
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            var item = _context.CartItems
                .FirstOrDefault(c => c.UserId == user.Id && c.ProductId == id);

            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    _context.CartItems.Remove(item); // 1 ise ve azaltılırsa sil
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // --- SEPETTEN SİL ---
        [HttpPost]
        public IActionResult Remove(int id) // id burada ProductId
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            var item = _context.CartItems
                .FirstOrDefault(c => c.UserId == user.Id && c.ProductId == id);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}