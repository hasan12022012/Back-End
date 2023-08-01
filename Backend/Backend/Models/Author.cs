namespace Backend.Models
{
    public class Author : BaseEntity
    {
        public string? Name { get; set; }
        public string? About { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
