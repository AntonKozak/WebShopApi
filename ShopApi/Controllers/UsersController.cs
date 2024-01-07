using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Extensions;
using ShopApi.Interfaces;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
    
    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
        _photoService = photoService;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var  users = await _userRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUserByUserName(string username)
    {
        return await _userRepository.GetMemberAsync(username);  
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult<UserModel>> GetUserById(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
         // in the static class ClaimsPrincipalExtensions extensions GetUsername()
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return NotFound();
        _mapper.Map(memberUpdateDto, user);

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user nothing was saved");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<UsersPhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return NotFound();
        var result = await _photoService.AddPhotoAsync(file);
        if(result.Error != null) return BadRequest(result.Error.Message);
        var photo = new UsersPhoto
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        if(user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }
        user.Photos.Add(photo);
        if(await _userRepository.SaveAllAsync())
        {
            return _mapper.Map<UsersPhotoDto>(photo);
            // return CreatedAtRoute("GetUserByUserName", new {username = user.UserName}, _mapper.Map<UsersPhotoDto>(photo));
        }
        return BadRequest("Problem adding photo");
    }
}
