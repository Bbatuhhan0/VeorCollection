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

        // --- GÜNCELLENEN INDEX METODU (Ana Sayfa) ---
        public IActionResult Index()
        {
            // Veritabanından en son eklenen 8 ürünü çekiyoruz.
            // OrderByDescending(p => p.Id) -> En yüksek ID (en yeni) en üstte gelir.
            var recentProducts = _context.Products
                                         .OrderByDescending(p => p.Id)
                                         .Take(8)
                                         .ToList();

            // Listeyi View'a gönderiyoruz.
            return View(recentProducts);
        }

        // --- ÜRÜNLER LİSTESİ ---
        // --- GÜNCELLENEN PRODUCTS METODU (Sayfalama Destekli) ---
        public IActionResult Products(int? categoryId, List<int>? f, int page = 1)
        {
            // 1. Temel Sorgu
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

            // 3. Özellik Filtresi
            if (f != null && f.Any())
            {
                var selectedValues = _context.ProductAttributeValues
                    .Where(v => f.Contains(v.Id))
                    .Select(v => new { v.Id, v.ProductAttributeId })
                    .ToList();

                var groupedFilters = selectedValues.GroupBy(v => v.ProductAttributeId).ToList();

                foreach (var group in groupedFilters)
                {
                    var groupValueIds = group.Select(x => x.Id).ToList();
                    productsQuery = productsQuery.Where(p => p.AttributeValues.Any(av => groupValueIds.Contains(av.Id)));
                }
                ViewBag.SelectedFilters = f;
            }

            // 4. SAYFALAMA MANTIĞI (YENİ EKLENEN KISIM)
            int pageSize = 9; // Her sayfada kaç ürün olacak?
            var totalItems = productsQuery.Count(); // Toplam ürün sayısı
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Geçersiz sayfa numarası önlemi
            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var pagedProducts = productsQuery
                .OrderByDescending(p => p.Id) // En yeniler en üstte
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // View'a Sayfalama Bilgilerini Gönder
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // 5. Sidebar Özelliklerini Hazırla
            List<ProductAttribute> attributesToShow = new List<ProductAttribute>();
            if (categoryId.HasValue)
            {
                attributesToShow = _context.ProductAttributes
                    .Include(a => a.Values)
                    .Include(a => a.CategoryAttributes)
                    .Where(a => !a.CategoryAttributes.Any() || a.CategoryAttributes.Any(ca => ca.CategoryId == categoryId.Value))
                    .ToList();
            }

            ViewBag.Attributes = attributesToShow;
            ViewBag.Categories = _context.Categories.ToList();

            return View(pagedProducts);
        }

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

        public IActionResult Blog()
        {
            var blogs = _context.Blogs.OrderByDescending(b => b.CreatedDate).ToList();
            return View(blogs);
        }

        public IActionResult BlogDetail(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog == null) return RedirectToAction("Blog");

            ViewBag.RecentBlogs = _context.Blogs
                .Where(b => b.Id != id)
                .OrderByDescending(b => b.CreatedDate)
                .Take(3)
                .ToList();

            return View("blogdetail", blog);
        }

        public IActionResult About() { return View(); }
    }
}