using Backend.Models;

namespace Backend.ViewModels
{
    public class BlogVM
    {
        public IEnumerable<Blog>? Blogs { get; set; }
        public IEnumerable<BlogCategory>? BlogCategories { get; set; }
        public IEnumerable<Tag>? Tags { get; set; }
        public IEnumerable<BlogComment>? BlogComments { get; set; }
        public Blog? Blog { get; set; }
    }
}
