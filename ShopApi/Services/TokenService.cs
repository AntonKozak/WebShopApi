
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Interfaces;

namespace ShopApi.Services;

public class TokenService : ITokenService
{

    private readonly SymmetricSecurityKey _key;
    public TokenService(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]??throw new System.Exception("Token key not found. TOKEN SERVICE")));
    }
    public string CreateToken(UserModel user)
    {
        var Claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.UserName??throw new System.Exception("Username not found. TOKEN SERVICE"))
        };
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(Claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
