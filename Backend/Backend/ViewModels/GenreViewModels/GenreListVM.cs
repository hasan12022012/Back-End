using Backend.Models;

namespace Backend.ViewModels.GenreViewModels
{
    public class GenreListVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IEnumerable<ProductGenre>? Books { get; set; }
    }
}
