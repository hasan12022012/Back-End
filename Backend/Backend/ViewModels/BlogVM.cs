using Backend.Models;

namespace Backend.ViewModels
{
    public class BlogVM
    {
        public IEnumerable<Blog>? Blogs { get; set; }
    }
}
