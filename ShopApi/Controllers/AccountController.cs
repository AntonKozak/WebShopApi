using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.DTOs;
using ShopApi.Interfaces;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(UserManager<UserModel> userManager, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (registerDto.UserName == null) { return BadRequest("Username is required"); }

            if (UserExists(registerDto.UserName).Result) { return BadRequest("Username is taken"); }

            var user = _mapper.Map<UserModel>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password); if (!result.Succeeded) { return BadRequest(result.Errors); }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded) { return BadRequest(result.Errors); }

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName); if (user == null) { return Unauthorized("Invalid username or password"); }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password); if (!result) { return Unauthorized("Invalid username or password"); }

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                Country = user.Country,
                City = user.City,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}