using Backend.Models;

namespace Backend.ViewModels.ProductViewModels
{
    public class ProductVM
    {
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Genre>? Genres { get; set; }
        public IEnumerable<Author>? Authors { get; set; }
        public Product? Product { get; set; }
    }
}
