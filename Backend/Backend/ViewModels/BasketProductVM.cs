namespace Online_Shop___BackEnd.ViewModels.BasketViewModels
{
    public class BasketProductVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
    }
}
