
using Microsoft.AspNetCore.SignalR;
using ShopApi.Extensions;
using ShopApi.Interfaces;

namespace ShopApi.SignalR;

public class MessageHub:  Hub
{
        private readonly IMessageRepository _messageRepository;
    public MessageHub(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public override async Task OnConnectedAsync()
    {
        //get user name 
        var hhtpContext = Context.GetHttpContext();
        var otherUser = hhtpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    private string GetGroupName(string caller, string other)
    {   //return boolean value becouse of <0 ; atherwise return int value
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }


}
