using DropBoxMarket.Models;
using DropBoxMarket.Models.ViewModels;

namespace DropBoxMarket.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, CheckoutViewModel input, List<CartItem> cart);
        Task<ConfirmationViewModel?> GetConfirmationAsync(int orderId);
        Task<List<Order>> GetOrdersForUserAsync (string userId);  
    }
}