using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeorCollection.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; } // Soru işareti (?) ekledik

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }    // Soru işareti (?) ekledik

        public int Quantity { get; set; }
    }
}