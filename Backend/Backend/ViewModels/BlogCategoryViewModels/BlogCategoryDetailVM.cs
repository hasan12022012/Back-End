using Backend.Models;

namespace Backend.ViewModels.BlogCategoryViewModels
{
    public class BlogCategoryDetailVM
    {
        public string? Name { get; set; }
        public IEnumerable<Blog>? Blogs { get; set; }
    }
}
