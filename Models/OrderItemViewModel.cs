namespace DropBoxMarket.Models.ViewModels
{
    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
