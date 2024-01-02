
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using ShopApi.Data;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ErrorHandler : ControllerBase
{
    private readonly DataContext _context;
    public ErrorHandler(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuthError()
    {
        return Unauthorized("Invalid username or password");
    }

    [HttpGet("notfound")]
    public ActionResult<UserModel> GetNotFoundError()
    {
        var user = _context.Users.Find(-1);
        if (user == null)
        { return NotFound(); }
        return user;
    }

    [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

    [HttpGet("badrequest")]
    public ActionResult<string> GetBadRequestError()
    {
        return BadRequest("This was a bad request");
    }

}
