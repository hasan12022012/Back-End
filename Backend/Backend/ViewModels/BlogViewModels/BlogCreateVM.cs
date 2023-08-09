using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels.BlogViewModels
{
    public class BlogCreateVM
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public List<int>? TagIds { get; set; }
        [Required]
        public IFormFile? Photo { get; set; }
    }
}
