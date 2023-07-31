namespace Backend.Models
{
    public class Genre : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<ProductGenre>? ProductGenres { get; set; }
    }
}
