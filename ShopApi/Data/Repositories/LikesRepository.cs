
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Helpers;
using ShopApi.Helpers.FilterParams;
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

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var user = _dataContext.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _dataContext.UsersLikes.AsQueryable();

        if(likesParams.Predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            user = likes.Select(like => like.TargetUser);
        }
        if(likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
            user = likes.Select(like => like.SourceUser);
        }

        var likedUser = user.Select(user => new LikeDto
        {
            UserName = user.UserName,
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = user.City,
            Country = user.Country
        });

        return await PagedList<LikeDto>.CreateAsync(likedUser, likesParams.PageNumber, likesParams.PageSize);
    }

    public Task<UserModel> GetUserWithLikes(int userId)
    {
        return _dataContext.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
