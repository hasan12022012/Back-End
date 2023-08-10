namespace Backend.Models
{
    public class Blog : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int BlogCategoryId { get; set; }
        public BlogCategory? BlogCategory { get; set; }
        public List<BlogTag>? BlogTags { get; set; }
        public ICollection<BlogComment>? BlogComments { get; set; }
    }
}
