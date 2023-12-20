using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
        private readonly DataContext _context;
    public UsersController(DataContext context)
    {
        _context = context;
    } 

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
    {
        if(_context.Users.Count() == 0)
        {
            return NotFound();
        }
        return await _context.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUserById(int id)
    {   
        if(id == 0)
        {
            return BadRequest();
        }
        var user = await _context.Users.FindAsync(id);
        if(user == null)
        {
            return NotFound();
        }
        return user;
    }

}
