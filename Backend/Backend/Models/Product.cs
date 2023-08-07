namespace Backend.Models
{
    public class Product : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public double? Rating { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<ProductGenre>? ProductGenres { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
