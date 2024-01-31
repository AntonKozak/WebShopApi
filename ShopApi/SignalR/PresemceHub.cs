using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShopApi.Extensions;

namespace ShopApi.SignalR;

public class PresemceHub : Hub
{
    private readonly PresenceTracker _tracker;
    public PresemceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
    }
    public override async Task OnConnectedAsync()
    {
        await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        // all connected users will be able to see this message when a new user connects to the hub 
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();

        await Clients.All.SendAsync("GetOnlineUsers from On Connected", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

        // all connected users will be able to see this message when a user disconnects from the hub 
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers from disconnect", currentUsers);
        await base.OnDisconnectedAsync(exception);
    }
}
