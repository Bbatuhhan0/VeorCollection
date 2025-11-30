using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeorCollection.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı boş geçilemez.")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Resim Yolu")]
        public string ImageUrl { get; set; }

        // --- İLİŞKİ AYARLARI ---

        // Bu ürün hangi kategoriye ait? (Zorunlu alan)
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        // Kod içinde kategoriye nokta koyup ulaşmak için
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}