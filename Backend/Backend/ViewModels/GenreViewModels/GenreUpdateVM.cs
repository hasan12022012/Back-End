using Backend.Models;

namespace Backend.ViewModels.GenreViewModels
{
    public class GenreUpdateVM
    {
        public string? Name { get; set; }
        public List<int>? ProductIds { get; set; }
        public List<ProductGenre>? Products { get; set; }
    }
}
