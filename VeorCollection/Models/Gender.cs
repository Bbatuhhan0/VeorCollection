using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class Gender
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // Örn: Erkek, Kadın, Unisex

        // İlişki: Bir cinsiyetin birden fazla ürünü olabilir
        public List<Product>? Products { get; set; }
    }
}