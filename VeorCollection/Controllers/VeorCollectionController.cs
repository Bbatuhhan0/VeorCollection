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

        // --- GÜNCELLENEN PRODUCTS METODU ---
        public IActionResult Products(int? categoryId, List<int>? f)
        {
            // 1. Ürünleri ve özelliklerini çek (Burada değişiklik yok)
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.ProductAttribute)
                .AsQueryable();

            // 2. Kategori Filtresi
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(x => x.CategoryId == categoryId.Value);
                ViewBag.SeciliCategoryId = categoryId.Value;
            }

            // 3. Dinamik Özellik Filtresi (AND Mantığı)
            if (f != null && f.Any())
            {
                var selectedValues = _context.ProductAttributeValues
                    .Where(v => f.Contains(v.Id))
                    .Select(v => new { v.Id, v.ProductAttributeId })
                    .ToList();

                var groupedFilters = selectedValues
                    .GroupBy(v => v.ProductAttributeId)
                    .ToList();

                foreach (var group in groupedFilters)
                {
                    var groupValueIds = group.Select(x => x.Id).ToList();
                    productsQuery = productsQuery.Where(p => p.AttributeValues.Any(av => groupValueIds.Contains(av.Id)));
                }

                ViewBag.SelectedFilters = f;
            }

            // --- SIDEBAR ÖZELLİK AYARI (BURASI DEĞİŞTİ) ---

            // Varsayılan olarak listeyi BOŞ başlatıyoruz. 
            // Böylece "Tüm Ürünler" seçiliyken hiçbir filtre görünmez.
            List<ProductAttribute> attributesToShow = new List<ProductAttribute>();

            // SADECE bir kategori seçildiyse filtreleri doldur
            if (categoryId.HasValue)
            {
                attributesToShow = _context.ProductAttributes
                    .Include(a => a.Values)
                    .Include(a => a.CategoryAttributes)
                    .Where(a =>
                        !a.CategoryAttributes.Any() || // Genel özellikler (her yerde görünsün diyenler)
                        a.CategoryAttributes.Any(ca => ca.CategoryId == categoryId.Value)) // Bu kategoriye özel olanlar
                    .ToList();
            }

            // View'a gönder
            ViewBag.Attributes = attributesToShow;
            ViewBag.Categories = _context.Categories.ToList();

            return View(productsQuery.ToList());
        }

        // Ürün Detay Sayfası
        public IActionResult ShopDetail(int id)
        {
            if (id <= 0) return RedirectToAction("Products");

            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.ProductAttribute)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return RedirectToAction("Products");

            return View(product);
        }

        public IActionResult Blog() { return View(); }
        public IActionResult About() { return View(); }

        // Seed Data
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