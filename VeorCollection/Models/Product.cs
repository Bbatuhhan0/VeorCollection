namespace VeorCollection.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }      // Ürün Adı
        public string Description { get; set; } // Açıklama
        public decimal Price { get; set; }    // Fiyat
        public string ImageUrl { get; set; }  // Resim Yolu (Örn: ~/html/assets/images/resource/products/1.png)
        public string Category { get; set; }  // Kategori (Erkek, Kadın)
        public bool IsFeatured { get; set; }  // Öne çıkan ürün mü?
    }
}