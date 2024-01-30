using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.DTOs;
using ShopApi.Interfaces;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public ActionResult<UserDto> Register(RegisterDto registerDto)
        {   if(registerDto.UserName == null)
            {
                return BadRequest("Username is required");
            }

            if(UserExists(registerDto.UserName).Result)
            {
                return BadRequest("Username is taken");
            }
            
            var user = _mapper.Map<UserModel>(registerDto);


            
                user.UserName = registerDto.UserName?.ToLower();
                user.Email = registerDto.Email;
                user.FirstName = registerDto.FirstName;
                user.LastName = registerDto.LastName;
                user.Country = registerDto.Country;
                user.City = registerDto.City;
            
            _context.Users.Add(user);
            _context.SaveChanges();
            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);
            if(user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                Country = user.Country,
                City = user.City,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}