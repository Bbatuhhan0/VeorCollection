using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class ProductAttribute
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Özellik Adı")]
        public string Name { get; set; } = string.Empty; // Hata veren kısım düzeltildi

        public virtual ICollection<ProductAttributeValue> Values { get; set; } = new List<ProductAttributeValue>();

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; } = new List<CategoryAttribute>();
    }
}