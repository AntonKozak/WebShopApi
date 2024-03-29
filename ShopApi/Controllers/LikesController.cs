
using Microsoft.AspNetCore.Mvc;
using ShopApi.DTOs;
using ShopApi.Entities;
using ShopApi.Extensions;
using ShopApi.Helpers;
using ShopApi.Helpers.FilterParams;
using ShopApi.Interfaces;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/likes")]
public class LikesController : ControllerBase
{
        private readonly IUnitOfWork _unitOfWork;

public LikesController(IUnitOfWork unitOfWork)
{
    _unitOfWork = unitOfWork;
    
}

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();//return with the id of the user that is logged in
        var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);//set likes to the user
        var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);//loged in, current user

        if(likedUser == null) return NotFound();

        if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if(userLike != null) return BadRequest("You already like this user");

        userLike = new UsersLikes
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if(await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to like user");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
    {   
        likesParams.UserId = User.GetUserId();
        var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);
        
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }
}
