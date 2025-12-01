using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty; // Başlangıç değeri atadık

        [Display(Name = "Açıklama")]
        public string? Description { get; set; } // ? koyarak boş olabilir dedik

        // İlişki - Başlangıçta boş liste olsun
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}