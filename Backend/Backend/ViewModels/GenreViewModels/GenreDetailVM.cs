using Backend.Models;

namespace Backend.ViewModels.GenreViewModels
{
    public class GenreDetailVM
    {
        public string? Name { get; set; }
        public List<ProductGenre>? Products { get; set; }
    }
}
