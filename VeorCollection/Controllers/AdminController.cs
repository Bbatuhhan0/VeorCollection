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
using System.Collections.Generic;

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
            // Not: TotalStockValue hesabı null kontrolü ile daha güvenli hale getirildi
            ViewBag.TotalStockValue = _context.Products.Any() ? _context.Products.Sum(p => p.Price) : 0;
            ViewBag.LowStockCount = _context.Products.Count(p => !p.IsInStock);

            return View();
        }

        // --- KATEGORİ İŞLEMLERİ ---
        public IActionResult Categories()
        {
            return View(_context.Categories.ToList());
        }

        [HttpGet]
        public IActionResult CreateCategory() { return View(); }

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

        // --- ÖZELLİK (FİLTRE) YÖNETİMİ ---

        // GÜNCELLENDİ: Özellik Yönetimi Sayfası
        public IActionResult Attributes()
        {
            // Kategorileri de View'a gönderelim ki seçim yapabilelim
            ViewBag.Categories = _context.Categories.ToList();

            var attributes = _context.ProductAttributes
                .Include(a => a.Values)
                .Include(a => a.CategoryAttributes) // Eşleşmeleri de getir
                    .ThenInclude(ca => ca.Category)
                .ToList();

            return View(attributes);
        }

        // GÜNCELLENDİ: Yeni Özellik Ekle (Kategori Seçimli)
        [HttpPost]
        public IActionResult CreateAttribute(string name, int[] categoryIds)
        {
            if (string.IsNullOrEmpty(name)) return RedirectToAction("Attributes");

            var attr = new ProductAttribute { Name = name };
            _context.ProductAttributes.Add(attr);
            _context.SaveChanges(); // ID oluşsun

            // Eğer kategori seçildiyse eşleştirmeleri kaydet
            if (categoryIds != null && categoryIds.Length > 0)
            {
                foreach (var catId in categoryIds)
                {
                    _context.CategoryAttributes.Add(new CategoryAttribute
                    {
                        ProductAttributeId = attr.Id,
                        CategoryId = catId
                    });
                }
                _context.SaveChanges();
            }

            TempData["Message"] = "Özellik eklendi ve kategorilere bağlandı.";
            TempData["Type"] = "success";
            return RedirectToAction("Attributes");
        }

        // Özelliğe Değer Ekle (Örn: Kırmızı)
        [HttpPost]
        public IActionResult AddAttributeValue(int attributeId, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _context.ProductAttributeValues.Add(new ProductAttributeValue
                {
                    ProductAttributeId = attributeId,
                    Value = value
                });
                _context.SaveChanges();
                TempData["Message"] = "Seçenek başarıyla eklendi.";
                TempData["Type"] = "success";
            }
            return RedirectToAction("Attributes");
        }

        public IActionResult DeleteAttribute(int id)
        {
            var attr = _context.ProductAttributes.Find(id);
            if (attr != null) { _context.ProductAttributes.Remove(attr); _context.SaveChanges(); }
            return RedirectToAction("Attributes");
        }

        public IActionResult DeleteAttributeValue(int id)
        {
            var val = _context.ProductAttributeValues.Find(id);
            if (val != null) { _context.ProductAttributeValues.Remove(val); _context.SaveChanges(); }
            return RedirectToAction("Attributes");
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
            LoadViewBags(); // Özellik listesini yükleyen metot
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile, int[] selectedOptions)
        {
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var location = Path.Combine(folderPath, newImageName);
                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/img/products/" + newImageName;
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Seçilen özellikleri kaydet
                if (selectedOptions != null)
                {
                    foreach (var valId in selectedOptions)
                    {
                        var attrVal = await _context.ProductAttributeValues.FindAsync(valId);
                        if (attrVal != null) product.AttributeValues.Add(attrVal);
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Products");
            }

            LoadViewBags();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.AttributeValues)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            LoadViewBags(product);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile, int[] selectedOptions)
        {
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var location = Path.Combine(folderPath, newImageName);
                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/img/products/" + newImageName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productToUpdate = await _context.Products
                        .Include(p => p.AttributeValues)
                        .FirstOrDefaultAsync(p => p.Id == product.Id);

                    if (productToUpdate == null) return NotFound();

                    // Alanları güncelle
                    productToUpdate.Name = product.Name;
                    productToUpdate.Price = product.Price;
                    productToUpdate.ShortDescription = product.ShortDescription;
                    productToUpdate.FullDescription = product.FullDescription;
                    productToUpdate.SKU = product.SKU;
                    productToUpdate.IsInStock = product.IsInStock;
                    productToUpdate.CategoryId = product.CategoryId;
                    if (product.ImageUrl != null) productToUpdate.ImageUrl = product.ImageUrl;

                    // Özellikleri güncelle
                    productToUpdate.AttributeValues.Clear();
                    if (selectedOptions != null)
                    {
                        foreach (var valId in selectedOptions)
                        {
                            var attrVal = await _context.ProductAttributeValues.FindAsync(valId);
                            if (attrVal != null) productToUpdate.AttributeValues.Add(attrVal);
                        }
                    }

                    _context.Update(productToUpdate);
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

        // --- YARDIMCI METOT ---
        private void LoadViewBags(Product? product = null)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product?.CategoryId);

            // Tüm Özellikleri ve Değerlerini View'a gönderiyoruz
            ViewBag.Attributes = _context.ProductAttributes.Include(a => a.Values).ToList();
        }

        // --- BLOG YÖNETİMİ ---

        // --- BLOG YÖNETİMİ (GÜNCELLENDİ) ---

        public IActionResult Blogs()
        {
            var blogs = _context.Blogs.OrderByDescending(b => b.CreatedDate).ToList();
            return View(blogs);
        }

        [HttpGet]
        public IActionResult CreateBlog()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Güvenlik önlemi
        public async Task<IActionResult> CreateBlog(Blog blog, IFormFile? imageFile)
        {
            if (imageFile != null)
            {
                // 1. Dosya Uzantı Kontrolü (Sadece resim dosyaları)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageUrl", "Sadece .jpg, .jpeg, .png veya .webp formatında resim yükleyebilirsiniz.");
                    return View(blog);
                }

                // 2. Benzersiz Dosya Adı
                var newImageName = Guid.NewGuid() + extension;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/blog");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var location = Path.Combine(folderPath, newImageName);
                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                blog.ImageUrl = "/img/blog/" + newImageName;
            }

            if (ModelState.IsValid)
            {
                blog.CreatedDate = DateTime.Now;
                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Blog başarıyla eklendi."; // Kullanıcıya mesaj
                return RedirectToAction("Blogs");
            }
            return View(blog);
        }

        public IActionResult DeleteBlog(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog != null)
            {
                // 1. Önce Resmi Sunucudan Sil (Çöp dosya oluşmaması için)
                if (!string.IsNullOrEmpty(blog.ImageUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", blog.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // 2. Veritabanından Sil
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
            }
            return RedirectToAction("Blogs");
        }
    }
}