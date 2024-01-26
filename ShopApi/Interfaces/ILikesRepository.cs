
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Helpers;
using ShopApi.Helpers.FilterParams;

namespace ShopApi.Interfaces;

public interface ILikesRepository
{
    Task<UsersLikes> GetUserLike(int sourceUserId, int targetUserId);
    Task<UserModel> GetUserWithLikes(int userId);
    //predicate is for filtering likes by liker or likee
    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
}
