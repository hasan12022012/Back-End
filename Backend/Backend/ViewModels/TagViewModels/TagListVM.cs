using Backend.Models;

namespace Backend.ViewModels.TagViewModels
{
    public class TagListVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IEnumerable<BlogTag>? Blogs { get; set; }
    }
}
