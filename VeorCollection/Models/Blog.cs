using System;
using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [Display(Name = "Blog Başlığı")]
        public string Title { get; set; } = string.Empty; // <-- DÜZELTİLDİ: Başlangıç değeri atandı

        [Display(Name = "Kısa Açıklama (Listede görünür)")]
        [Required(ErrorMessage = "Kısa açıklama zorunludur.")]
        [StringLength(300, ErrorMessage = "Kısa açıklama en fazla 300 karakter olabilir.")]
        public string ShortDescription { get; set; } = string.Empty; // <-- DÜZELTİLDİ

        [Display(Name = "Kapak Resmi")]
        public string? ImageUrl { get; set; } // '?' olduğu için buna değer atamaya gerek yok, boş kalabilir.

        [Required(ErrorMessage = "Kategori (Etiket) zorunludur.")]
        [Display(Name = "Ana Etiket (Örn: Moda)")]
        public string MainTag { get; set; } = string.Empty; // <-- DÜZELTİLDİ

        [Display(Name = "Diğer Etiketler (Virgülle ayırın)")]
        public string? OtherTags { get; set; }

        [Required(ErrorMessage = "İçerik zorunludur.")]
        [Display(Name = "Blog İçeriği")]
        public string Content { get; set; } = string.Empty; // <-- DÜZELTİLDİ

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string Author { get; set; } = "Veor Editör"; // Buna zaten değer atamışsınız, bu doğru.
    }
}