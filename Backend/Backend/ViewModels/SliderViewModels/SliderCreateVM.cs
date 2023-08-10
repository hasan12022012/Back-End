using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels.SliderViewModels
{
    public class SliderCreateVM
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public IFormFile? Photo { get; set; }
    }
}
