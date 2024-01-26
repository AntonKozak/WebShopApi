using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Extensions;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Interfaces;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/likes")]
public class MessagesController: ControllerBase
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

        if(username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself");
        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if(recipient == null) return NotFound();
        
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        
        _messageRepository.AddMessage(message);

        if(await _messageRepository.SaveAllAsync())
        return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message MessagesController");
    }

}
