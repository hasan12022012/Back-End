namespace Backend.Models
{
    public class Tag : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<BlogTag>? BlogTags { get; set; }
    }
}
