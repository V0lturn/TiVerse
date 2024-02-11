using Microsoft.AspNetCore.SignalR;
using TiVerse.Application.ViewModels;

namespace TiVerse.Application.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendTaskCreatedNotification(RouteViewModel model)
        {
            await Clients.All.SendAsync("Route created", model);
        }
    }
}
