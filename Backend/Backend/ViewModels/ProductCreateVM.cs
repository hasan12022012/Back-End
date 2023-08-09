using Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels
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
        public List<Genre>? Genres { get; set; }
        [Required]
        public List<IFormFile>? Photos { get; set; }
    }
}
