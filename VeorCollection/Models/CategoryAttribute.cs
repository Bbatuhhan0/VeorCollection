using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeorCollection.Models
{
    // Çoka-Çok İlişki Tablosu (Hangi Özellik - Hangi Kategori)
    public class CategoryAttribute
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public int ProductAttributeId { get; set; }
        [ForeignKey("ProductAttributeId")]
        public virtual ProductAttribute ProductAttribute { get; set; }
    }
}