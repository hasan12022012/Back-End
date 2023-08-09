using Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int AuthorId { get; set; }
        public string? Author { get; set; }
        public List<ProductGenre>? GenreName { get; set; }
        public List<Genre>? Genres { get; set; }
        public List<int>? GenreIds { get; set; }
        public ICollection<ProductImage>? Images { get; set; }
        public List<IFormFile>? Photos { get; set; }
    }
}
