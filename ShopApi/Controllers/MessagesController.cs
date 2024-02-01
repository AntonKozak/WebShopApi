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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        //get the username of the user that is logged in from the token credentials 
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself");
        var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        _unitOfWork.MessageRepository.AddMessage(message);

        if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message MessagesController");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _unitOfWork.MessageRepository.GetMessage(id);
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
            _unitOfWork.MessageRepository.DeleteMessage(message);
        }

        if (await _unitOfWork.Complete())
        {
            return Ok();
        }

        return BadRequest("Failed to delete message");
    }

}
