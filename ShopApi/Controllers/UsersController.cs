using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Extensions;
using ShopApi.Helpers;
using ShopApi.Interfaces;

namespace ShopApi.Controllers;

[Authorize (Roles = "Admin, Moderator, User")]
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
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams filteringsParams)
    {
        var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        filteringsParams.CurrentUsername = currentUser.UserName;
        if(string.IsNullOrEmpty(filteringsParams.Country))
        {
            filteringsParams.Country = currentUser.Country;
        }

        var users = await _userRepository.GetMembersAsync(filteringsParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
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
            return CreatedAtAction(nameof(GetUserByUserName), new {username = user.UserName}, _mapper.Map<UsersPhotoDto>(photo));
        }
        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if(photo.IsMain) return BadRequest("This is already your main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if(currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;
        
        if(await _userRepository.SaveAllAsync()) return NoContent();
        return BadRequest("Failed to set main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if(photo == null) return NotFound();

        if(photo.IsMain) return BadRequest("You cannot delete your main photo");

        if(photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);
        if(await _userRepository.SaveAllAsync()) return Ok();
        return BadRequest("Failed to delete photo");
    }
}
