using Backend.Models;

namespace Backend.ViewModels.BlogViewModels
{
    public class BlogUpdateVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
        public List<BlogTag>? Tags { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
