namespace DropBoxMarket.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; } 
        public string Address { get; set; } = string.Empty;
    }
}
