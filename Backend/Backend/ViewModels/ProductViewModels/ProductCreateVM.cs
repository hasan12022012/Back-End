using Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels.ProductViewModels
{
    public class ProductCreateVM
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public List<int>? GenreIds { get; set; }
        [Required]
        public List<IFormFile>? Photos { get; set; }
    }
}
