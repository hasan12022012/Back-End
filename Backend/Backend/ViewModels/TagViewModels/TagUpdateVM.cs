using Backend.Models;

namespace Backend.ViewModels.TagViewModels
{
    public class TagUpdateVM
    {
        public string? Name { get; set; }
        public List<int>? BlogIds { get; set; }
        public List<BlogTag>? Blogs { get; set; }
    }
}
