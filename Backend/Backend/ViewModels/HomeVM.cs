using Backend.Models;

namespace Backend.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider>? Sliders { get; set; }
        public IEnumerable<Product>? Products { get; set; }
    }
}
