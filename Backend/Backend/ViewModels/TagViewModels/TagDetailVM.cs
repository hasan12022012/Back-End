using Backend.Models;

namespace Backend.ViewModels.TagViewModels
{
    public class TagDetailVM
    {
        public string? Name { get; set; }
        public List<BlogTag>? Blogs { get; set; }
    }
}
