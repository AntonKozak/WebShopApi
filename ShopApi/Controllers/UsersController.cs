using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.DTOs;
using ShopApi.Interfaces;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
    
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
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



}
