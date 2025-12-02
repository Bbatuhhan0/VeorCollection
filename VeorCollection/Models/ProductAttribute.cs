using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class ProductAttribute
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Özellik Adı")] // Örn: Koku Tipi, Materyal, Renk
        public string Name { get; set; }

        // Bu özelliğe ait değerler (Örn: Koku Tipi -> Odunsu, Çiçeksi)
        public virtual ICollection<ProductAttributeValue> Values { get; set; }
    }
}