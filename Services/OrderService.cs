using DropBoxMarket.Data;
using DropBoxMarket.Hubs;
using DropBoxMarket.Models;
using DropBoxMarket.Models.ViewModels;
using DropBoxMarket.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DropBoxMarket.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<OrderHub> _hub;

        public OrderService(ApplicationDbContext context, IHubContext<OrderHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<int> CreateOrderAsync(string userId, CheckoutViewModel input, List<CartItem> cart)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException("userId is required");
            }
            if (cart == null || cart.Count == 0) throw new InvalidOperationException("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                FullName = input.FullName,
                Phone = input.Phone,
                Address = input.Address,
                Items = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group("Admins").SendAsync("ReceiveOrderNotification", new
            {
                Message = $"Нова поръчка от {order.FullName}",
                orderId = order.Id,

            });

            return order.Id;
        }

        public async Task<ConfirmationViewModel?> GetConfirmationAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            return new ConfirmationViewModel
            {
                FullName = order.FullName,
                OrderDate = order.OrderDate,
                TotalPrice = order.Items.Sum(i => i.Quantity * i.Product.Price),
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.Product.Title,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()

            };
        }


        public async Task<List<Order>> GetOrdersForUserAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
