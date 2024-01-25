
using ShopApi.DTOs;
using ShopApi.Entities;

namespace ShopApi.Interfaces;

public interface ILikesRepository
{
    Task<UsersLikes> GetUserLike(int sourceUserId, int likedUserId);
    Task<UserModel> GetUserWithLikes(int userId);
    //predicate is for filtering likes by liker or likee
    Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
}
