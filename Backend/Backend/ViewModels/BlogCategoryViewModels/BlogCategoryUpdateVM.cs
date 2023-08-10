using Backend.Models;

namespace Backend.ViewModels.BlogCategoryViewModels
{
    public class BlogCategoryUpdateVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<int>? BlogIds { get; set; }
        public List<Blog>? Blogs { get; set; }
    }
}
