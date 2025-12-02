using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeorCollection.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Resim")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Kısa Açıklama (Fiyat Altı)")]
        public string? ShortDescription { get; set; }

        [Display(Name = "Ürün Detayları (Uzun Açıklama)")]
        public string? FullDescription { get; set; }

        [Display(Name = "Stok Kodu (SKU)")]
        public string? SKU { get; set; }

        [Display(Name = "Stokta Var mı?")]
        public bool IsInStock { get; set; } = true;

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // --- YENİ EKLENEN KISIM ---
        // Ürünün sahip olduğu tüm özellikler (Hem Koku, Hem Renk, Hem Beden...)
        public virtual ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    }
}