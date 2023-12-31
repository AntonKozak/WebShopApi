using System.Security.Cryptography;
using System.Text;
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
        public AccountController(DataContext context, ITokenService tokenService)
        {
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
            

            using var hmac = new HMACSHA512();
            var user = new UserModel
            {
                UserName = registerDto.UserName?.ToLower(),
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Country = registerDto.Country,
                City = registerDto.City,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password??"password")),
                PasswordSalt = hmac.Key
            };
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
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);
            if(user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt??new byte[0]);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password??"password"));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash?[i])
                {
                    return Unauthorized("Invalid password or username or server error ");
                }
            }
            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}