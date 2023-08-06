namespace Backend.Models
{
    public class Contact : BaseEntity
    {
        public string? Message { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
    }
}
