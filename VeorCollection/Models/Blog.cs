using System;
using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [Display(Name = "Blog Başlığı")]
        public string Title { get; set; }

        [Display(Name = "Kapak Resmi")]
        public string ImageUrl { get; set; } // Resim dosya yolu

        [Required(ErrorMessage = "Ana Etiket (Kategori) zorunludur.")]
        [Display(Name = "Ana Etiket (Örn: Moda, Trend)")]
        public string MainTag { get; set; } // Listeleme sayfasında resmin üzerindeki etiket

        [Display(Name = "Diğer Etiketler (Virgülle ayırın)")]
        public string? OtherTags { get; set; } // Detay sayfasındaki etiketler

        [Required(ErrorMessage = "İçerik zorunludur.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } // HTML Editörden gelecek uzun yazı

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Author { get; set; } = "Admin";
    }
}