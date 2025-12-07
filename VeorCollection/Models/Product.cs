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
        [Column(TypeName = "decimal(18,2)")] // Hassas para birimi formatı
        public decimal Price { get; set; }

        [Display(Name = "Resim")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Kısa Açıklama")]
        public string? ShortDescription { get; set; }

        [Display(Name = "Ürün Detayları")]
        public string? FullDescription { get; set; }

        [Display(Name = "Stok Kodu (SKU)")]
        public string? SKU { get; set; }

        [Display(Name = "Stok Durumu")]
        public bool IsInStock { get; set; } = true;

        // Otomatik tarih ataması
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    }
}