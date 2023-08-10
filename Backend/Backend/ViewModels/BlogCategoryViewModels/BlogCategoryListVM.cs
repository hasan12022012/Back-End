using Backend.Models;

namespace Backend.ViewModels.BlogCategoryViewModels
{
    public class BlogCategoryListVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IEnumerable<Blog>? Blogs { get; set; }
    }
}
