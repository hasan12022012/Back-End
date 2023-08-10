namespace Backend.Models
{
    public class BlogComment
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string? Message { get; set; }
        public DateTime CreateDate { get; set; }
        public int? BlogId { get; set; }
        public Blog? Blog { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
