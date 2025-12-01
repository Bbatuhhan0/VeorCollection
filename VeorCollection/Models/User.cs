using System.ComponentModel.DataAnnotations;

namespace VeorCollection.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Uye";
    }
}