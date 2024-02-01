using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShopApi.Extensions;

namespace ShopApi.SignalR;

public class PresenceHub : Hub
{
    private readonly PresenceTracker _tracker;
    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
    }
    public override async Task OnConnectedAsync()
    {
        // all connected users will be able to see this message when a new user connects to the hub 
        var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        if(isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsersFromAPI", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        // all connected users will be able to see this message when a user disconnects from the hub 
        // send new list how is online
        if(isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

        await base.OnDisconnectedAsync(exception);
    }
}
