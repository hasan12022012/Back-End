namespace Backend.ViewModels.SliderViewModels
{
    public class SliderUpdateVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
