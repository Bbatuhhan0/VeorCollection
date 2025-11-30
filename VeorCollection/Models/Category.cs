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
        public string Name { get; set; }

        // Bir kategoride birden fazla ürün olabilir
        public virtual ICollection<Product> Products { get; set; }
    }
}