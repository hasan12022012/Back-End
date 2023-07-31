namespace Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string? Message { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public Rating? Rating { get; set; }
    }
}
