using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DropBoxMarket.Hubs
{
    public class OrderHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Administrator"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            await base.OnConnectedAsync();
        }
    }
}
