using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Extensions;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Interfaces;
using ShopApi.Helpers.FilterParams;
using ShopApi.Helpers;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    public MessagesController(
    IUserRepository userRepository,
    IMessageRepository messageRepository,
    IMapper mapper
    )
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        //get the username of the user that is logged in from the token credentials 
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself");
        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message MessagesController");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await _messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();
        return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _messageRepository.GetMessage(id);
        if (message == null)
        {
            return NotFound();
        }

        if (message.SenderUsername != username && message.RecipientUsername != username)
        {
            return Unauthorized();
        }

        // if the sender deleted the message
        if (message.SenderUsername == username)
        {
            message.SenderDeleted = true;
        }

        // if the recipient deleted the message
        if (message.RecipientUsername == username)
        {
            message.RecipientDeleted = true;
        }

        // if both the sender and the recipient deleted the message
        if (message.SenderDeleted && message.RecipientDeleted)
        {
            _messageRepository.DeleteMessage(message);
        }

        if (await _messageRepository.SaveAllAsync())
        {
            return Ok();
        }

        return BadRequest("Failed to delete message");
    }

}
