using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using VeorCollection.Data;
using VeorCollection.Models;

namespace VeorCollection.Controllers
{
    public class VeorCollectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeorCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // --- GÜNCELLENEN DİNAMİK FİLTRELEME ---
        // categoryId: Seçilen kategori
        // f: Seçilen özelliklerin ID listesi (Örn: [KırmızıID, XLID]) - Checkboxlardan gelen veri
        public IActionResult Products(int? categoryId, List<int>? f)
        {
            // 1. Ürünleri ve dinamik özelliklerini çekiyoruz
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.ProductAttribute) // Özellik detaylarını da al
                .AsQueryable();

            // 2. Kategori Filtresi
            if (categoryId.HasValue)
            {
                products = products.Where(x => x.CategoryId == categoryId.Value);
                ViewBag.SeciliCategoryId = categoryId.Value;
            }

            // 3. Dinamik Özellik Filtresi (Çoklu Seçim)
            if (f != null && f.Any())
            {
                // Seçilen özelliklerden HERHANGİ BİRİNE sahip ürünleri getir
                products = products.Where(p => p.AttributeValues.Any(av => f.Contains(av.Id)));
                ViewBag.SelectedFilters = f;
            }

            // Sidebar'ı doldurmak için TÜM özellikleri View'a gönderiyoruz
            // (Artık Cinsiyet, Koku, Materyal vs. hepsi burada)
            ViewBag.Attributes = _context.ProductAttributes.Include(a => a.Values).ToList();
            ViewBag.Categories = _context.Categories.ToList();

            return View(products.ToList());
        }

        // Ürün Detay Sayfası
        public IActionResult ShopDetail(int id)
        {
            if (id <= 0) return RedirectToAction("Products");

            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.AttributeValues) // Özellikleri dahil et
                    .ThenInclude(av => av.ProductAttribute)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return RedirectToAction("Products");

            return View(product);
        }

        public IActionResult Blog() { return View(); }
        public IActionResult About() { return View(); }

        // Seed Data (Opsiyonel: İlk kurulumda örnek veri ekler)
        public IActionResult SeedData()
        {
            if (!_context.ProductAttributes.Any())
            {
                var genderAttr = new ProductAttribute { Name = "Cinsiyet" };
                _context.ProductAttributes.Add(genderAttr);
                _context.SaveChanges();

                _context.ProductAttributeValues.AddRange(
                    new ProductAttributeValue { Value = "Erkek", ProductAttributeId = genderAttr.Id },
                    new ProductAttributeValue { Value = "Kadın", ProductAttributeId = genderAttr.Id },
                    new ProductAttributeValue { Value = "Unisex", ProductAttributeId = genderAttr.Id }
                );
                _context.SaveChanges();
                return Content("Başarılı: Örnek özellikler eklendi.");
            }
            return Content("Bilgi: Veriler zaten var.");
        }
    }
}