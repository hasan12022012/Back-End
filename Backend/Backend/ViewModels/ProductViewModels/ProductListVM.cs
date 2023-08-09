namespace Backend.ViewModels.ProductViewModels
{
    public class ProductListVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? MainImage { get; set; }
    }
}
