using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels.GenreViewModels
{
    public class GenreCreateVM
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public List<int>? ProductIds { get; set; }
    }
}
