
namespace ShopApi.Interfaces;

public interface ITokenService
{
    string CreateToken(UserModel user);
}
