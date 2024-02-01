
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Extensions;
using ShopApi.Interfaces;

namespace ShopApi.SignalR;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly IUnitOfWork _unitOfWork;
    public MessageHub(IMapper mapper, IHubContext<PresenceHub> presenceHub, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _presenceHub = presenceHub;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        // Get the username from the HTTP context 
        var hhtpContext = Context.GetHttpContext();
        var otherUser = hhtpContext.Request.Query["user"];
        //We know that users in the same group
        // Determine the group name => based on user names
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        // Add the connection to the group and
        // Notify the group about the updated group
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", groupName);
        // Retrieve and send the existing message thread
        var messages = await _unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

        //use this value "ReceiveMessageThread" in Angular message.service.ts
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup();
        /// should be the same "UpdatedGroup" as in OnConnectedAsync method
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group.Name);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You cannot send messages to yourself. Did you get it?");

        var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Not found user Message hub SendMessage method");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        // Determine the group name for the message
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

        // Update message status based on group connections
        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        // if no connection, no notifications
        else
        {   // Notify the recipient about a new message if not in the group
            var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
            if (connections != null)
            {
                //"NewMessageReceived" should match in the angular message.service.ts
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = sender.UserName });
            }
        }

        _unitOfWork.MessageRepository.AddMessage(message);

        if (await _unitOfWork.Complete())
        {
            //use this value "NewMessage" in Angular message.service.ts to sent message throw SignalR
            // Here we send message to group of users
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private static string GetGroupName(string caller, string other)
    {
        // Determine the group name based on alphabetical order of user names
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        //return boolean value becouse of <0 ; atherwise return int value
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
        // Create a new connection with the current user's information
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            //adding a group if this name of group did not exist before
            _unitOfWork.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

        _unitOfWork.MessageRepository.RemoveConnection(connection);
        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to remove from group");
        //remove connection from DB and signalR disconnect self with OnDisconnectedAsync method
    }
}
