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
using System.Collections.Generic; // List kullanımı için gerekli

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

        // --- YENİ: DİNAMİK ÖZELLİK (FİLTRE) YÖNETİMİ ---
        // Buradan "Renk", "Materyal" gibi özellikler eklenecek
        public IActionResult Attributes()
        {
            // Özellikleri ve altındaki değerleri (Value) getiriyoruz
            var attributes = _context.ProductAttributes.Include(a => a.Values).ToList();
            return View(attributes);
        }

        // Yeni Özellik Grubu Ekle (Örn: Beden, Materyal)
        [HttpPost]
        public IActionResult CreateAttribute(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _context.ProductAttributes.Add(new ProductAttribute { Name = name });
                _context.SaveChanges();
            }
            return RedirectToAction("Attributes");
        }

        // Özelliğe Yeni Değer Ekle (Örn: Materyal -> Altın)
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
            }
            return RedirectToAction("Attributes");
        }

        // Özellik Grubunu Sil
        public IActionResult DeleteAttribute(int id)
        {
            var attr = _context.ProductAttributes.Find(id);
            if (attr != null)
            {
                _context.ProductAttributes.Remove(attr);
                _context.SaveChanges();
            }
            return RedirectToAction("Attributes");
        }

        // Özellik Değerini Sil
        public IActionResult DeleteAttributeValue(int id)
        {
            var val = _context.ProductAttributeValues.Find(id);
            if (val != null)
            {
                _context.ProductAttributeValues.Remove(val);
                _context.SaveChanges();
            }
            return RedirectToAction("Attributes");
        }

        // --- ÜRÜN İŞLEMLERİ (GÜNCELLENDİ) ---
        public IActionResult Products()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            LoadViewBags(); // Kategorileri ve Özellikleri yükler
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile, int[] selectedOptions)
        {
            // 1. Resim Yükleme İşlemi
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products");

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

            if (ModelState.IsValid)
            {
                // 2. Ürünü Kaydet (ID oluşması için önce kaydediyoruz)
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // 3. Seçilen Özellikleri (Checkbox) Kaydet
                if (selectedOptions != null && selectedOptions.Length > 0)
                {
                    foreach (var valId in selectedOptions)
                    {
                        var attrVal = await _context.ProductAttributeValues.FindAsync(valId);
                        if (attrVal != null)
                        {
                            // Özelliği ürüne ekle
                            product.AttributeValues.Add(attrVal);
                        }
                    }
                    // İlişkileri veritabanına yansıt
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
            // Ürünü, mevcut özellikleriyle birlikte getiriyoruz (Include)
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
            // 1. Resim Güncelleme
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

            // 2. Ürün Bilgilerini Güncelle
            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut ürünü veritabanından, ilişkili özellikleriyle çekiyoruz
                    var productToUpdate = await _context.Products
                        .Include(p => p.AttributeValues)
                        .FirstOrDefaultAsync(p => p.Id == product.Id);

                    if (productToUpdate == null) return NotFound();

                    // Basit alanları güncelle
                    productToUpdate.Name = product.Name;
                    productToUpdate.Price = product.Price;
                    productToUpdate.ShortDescription = product.ShortDescription;
                    productToUpdate.FullDescription = product.FullDescription;
                    productToUpdate.SKU = product.SKU;
                    productToUpdate.IsInStock = product.IsInStock;
                    productToUpdate.CategoryId = product.CategoryId;
                    if (product.ImageUrl != null) productToUpdate.ImageUrl = product.ImageUrl;

                    // 3. Özellikleri Güncelle (Eskileri temizle, yenileri ekle)
                    productToUpdate.AttributeValues.Clear(); // Önce temizle

                    if (selectedOptions != null)
                    {
                        foreach (var valId in selectedOptions)
                        {
                            var attrVal = await _context.ProductAttributeValues.FindAsync(valId);
                            if (attrVal != null)
                            {
                                productToUpdate.AttributeValues.Add(attrVal);
                            }
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

            // Tüm Özellikleri ve Değerlerini View'a gönderiyoruz (Checkbox listesi için)
            ViewBag.Attributes = _context.ProductAttributes.Include(a => a.Values).ToList();
        }
    }
}