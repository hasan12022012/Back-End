using Backend.Models;

namespace Backend.ViewModels.BlogViewModels
{
    public class BlogDetailVM
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public IEnumerable<BlogTag>? Tags { get; set; }
    }
}
