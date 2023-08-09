using Backend.Models;

namespace Backend.ViewModels.ProductViewModels
{
    public class ProductDetailVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? Author { get; set; }
        public IEnumerable<ProductGenre>? Genres { get; set; }
        public ICollection<ProductImage>? Images { get; set; }
    }
}
