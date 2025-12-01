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
        public string Name { get; set; } = string.Empty; // Başlangıç değeri

        [Required]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Resim Yolu")]
        public string? ImageUrl { get; set; } // ? ile boş olabilir izni verdik

        public bool IsInStock { get; set; } = true;

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; } // ? ekledik
    }
}