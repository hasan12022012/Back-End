using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels.TagViewModels
{
    public class TagCreateVM
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public List<int>? BlogIds { get; set; }
    }
}
