using Backend.Models;

namespace Backend.ViewModels.AuthorViewModels
{
    public class AuthorUpdateVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? About { get; set; }
        public List<int>? ProductIds { get; set; }
        public List<Product>? Products { get; set; }
    }
}
