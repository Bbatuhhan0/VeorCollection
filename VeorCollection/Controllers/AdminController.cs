using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeorCollection.Data;
using VeorCollection.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace VeorCollection.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- DASHBOARD ---
        public IActionResult Index()
        {
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.TotalStockValue = _context.Products.Any() ? _context.Products.Sum(p => p.Price) : 0;
            ViewBag.LowStockCount = _context.Products.Count(p => !p.IsInStock);

            return View();
        }

        // --- KATEGORİ İŞLEMLERİ ---
        public IActionResult Categories()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View(category);
        }

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

        // --- ÜRÜN İŞLEMLERİ ---
        public IActionResult Products()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            LoadViewBags();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile)
        {
            // --- HATA ÇÖZÜMÜ BURADA ---
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;

                // 1. Klasör yolunu tanımla
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products");

                // 2. Klasör yoksa OLUŞTUR (Bu satır hatayı çözer)
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 3. Dosya yolunu birleştir
                var location = Path.Combine(folderPath, newImageName);

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/img/products/" + newImageName;
            }
            // ---------------------------

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Products");
            }

            LoadViewBags();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            LoadViewBags(product);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
        {
            // --- HATA ÇÖZÜMÜ BURADA DA VAR ---
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products");

                // Klasör kontrolü ve oluşturma
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var location = Path.Combine(folderPath, newImageName);

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/img/products/" + newImageName;
            }
            // ---------------------------------

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Products");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
            }

            LoadViewBags(product);
            return View(product);
        }

        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Products");
        }

        private void LoadViewBags(Product? product = null)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product?.CategoryId);
            ViewBag.Genders = new SelectList(_context.Genders, "Id", "Name", product?.GenderId);
            ViewBag.ScentTypes = new SelectList(_context.ScentTypes, "Id", "Name", product?.ScentTypeId);
        }
    }
}