
namespace ShopApi.Interfaces;

public interface ITokenService
{
   Task<string> CreateToken(UserModel user);
}
