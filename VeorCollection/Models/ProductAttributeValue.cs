using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeorCollection.Models
{
    public class ProductAttributeValue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Değer")] // Örn: Odunsu, XL, Kırmızı
        public string Value { get; set; }

        // Hangi Özelliğe Ait?
        public int ProductAttributeId { get; set; }

        [ForeignKey("ProductAttributeId")]
        public virtual ProductAttribute ProductAttribute { get; set; }

        // Bu değere sahip ürünler (Çoka-Çok İlişki)
        public virtual ICollection<Product> Products { get; set; }
    }
}