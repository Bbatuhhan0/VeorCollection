using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class ScentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // Örn: Odunsu, Çiçeksi, Meyveli

        // İlişki: Bir koku tipinin birden fazla ürünü olabilir
        public List<Product>? Products { get; set; }
    }
}