namespace Feane.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string NameKey { get; set; }          // ключ для локализации
        public string DescriptionKey { get; set; }   // ключ для локализации
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }
}
