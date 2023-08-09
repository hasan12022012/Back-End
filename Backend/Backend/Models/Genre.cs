using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Genre : BaseEntity
    {
        public string? Name { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        public List<ProductGenre>? ProductGenres { get; set; }
    }
}
