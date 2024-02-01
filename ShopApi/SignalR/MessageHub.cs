
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
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _messageRepository = messageRepository;
    }

    public override async Task OnConnectedAsync()
    {
        //get user name 
        var hhtpContext = Context.GetHttpContext();
        var otherUser = hhtpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);
        //we know that users in the same group

        var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
        //use this value "ReceiveMessageThread" in Angular message.service.ts
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {   
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You cannot send messages to yourself. Did you get it?");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Not found user Message hub SendMessage method");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        //get ourgroup name
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _messageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            //use this value "NewMessage" in Angular message.service.ts to sent message throw SignalR
            // Here we send message to group of users
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private static string GetGroupName(string caller, string other)
    {   //return boolean value becouse of <0 ; atherwise return int value
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        //connection to the group if group exist
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            //adding a group if this name of group did not exist before
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await _messageRepository.SaveAllAsync();
    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await _messageRepository.GetConnection(Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        await _messageRepository.SaveAllAsync();
        //remove connection from DB and signalR disconnect self with OnDisconnectedAsync method
    }
}
