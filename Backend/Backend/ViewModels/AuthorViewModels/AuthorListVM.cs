using Backend.Models;

namespace Backend.ViewModels.AuthorViewModels
{
    public class AuthorListVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? About { get; set; }
        public IEnumerable<Product>? Products { get; set; }
    }
}
