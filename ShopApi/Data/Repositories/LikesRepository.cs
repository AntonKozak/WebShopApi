
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Interfaces;

namespace ShopApi.Data.Repositories;

public class LikesRepository : ILikesRepository
{
    public Task<UsersLikes> GetUserLike(int sourceUserId, int likedUserId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> GetUserWithLikes(int userId)
    {
        throw new NotImplementedException();
    }
}
