using Backend.Models;

namespace Backend.ViewModels.AuthorViewModels
{
    public class AuthorDetailVM
    {
        public string? Name { get; set; }
        public IEnumerable<Product>? Products { get; set; }
    }
}
