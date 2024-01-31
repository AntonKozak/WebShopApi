using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShopApi.Extensions;

namespace ShopApi.SignalR;

public class PresemceHub : Hub
{
    [Authorize]
    public override async Task OnConnectedAsync()
    {
        // all connected users will be able to see this message when a new user connects to the hub 
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // all connected users will be able to see this message when a user disconnects from the hub 
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
        await base.OnDisconnectedAsync(exception);
    }
}
