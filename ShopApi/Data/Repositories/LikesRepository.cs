
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Interfaces;

namespace ShopApi.Data.Repositories;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _dataContext;
    public LikesRepository(DataContext dataContext)
    {
        _dataContext = dataContext;

    }
    public async Task<UsersLikes> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _dataContext.UsersLikes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
    {
        var user = _dataContext.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _dataContext.UsersLikes.AsQueryable();

        if(predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == userId);
            user = likes.Select(like => like.TargetUser);
        }
        if(predicate == "likedBy")
        {
            likes = likes.Where(like => like.TargetUserId == userId);
            user = likes.Select(like => like.SourceUser);
        }

        return await user.Select(user => new LikeDto
        {
            UserName = user.UserName,
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = user.City,
            Country = user.Country
        }).ToListAsync();
    }

    public Task<UserModel> GetUserWithLikes(int userId)
    {
        return _dataContext.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
