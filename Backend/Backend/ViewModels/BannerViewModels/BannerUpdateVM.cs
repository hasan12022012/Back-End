namespace Backend.ViewModels.BannerViewModels
{
    public class BannerUpdateVM
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
