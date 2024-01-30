using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.DTOs;
using ShopApi.Entities;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    public AdminController(UserManager<UserModel> userManager, RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [Authorize(Policy = "AdminRole")]
    [HttpPost("edit-role")]
    public async Task<ActionResult> AddRoleToUser(AddRoleDto addRoleDto)
    {
        var user = await _userManager.FindByNameAsync(addRoleDto.Username);
        if (user == null) return NotFound("User not found");

        var role = await _roleManager.FindByNameAsync(addRoleDto.RoleName);
        if (role == null) return NotFound("Role not found");

        var result = await _userManager.AddToRoleAsync(user, addRoleDto.RoleName);
        if (!result.Succeeded) return BadRequest("Failed to add to role");

        return Ok();
    }

    [Authorize(Policy = "AdminRole")]
    [HttpPost("remove-role")]
    public async Task<ActionResult> RemoveRoleFromUser(AddRoleDto addRoleDto)
    {
        var user = await _userManager.FindByNameAsync(addRoleDto.Username);
        if (user == null) return NotFound("User not found");

        var role = await _roleManager.FindByNameAsync(addRoleDto.RoleName);
        if (role == null) return NotFound("Role not found");

        var result = await _userManager.RemoveFromRoleAsync(user, addRoleDto.RoleName);
        if (!result.Succeeded) return BadRequest("Failed to remove from role");

        return Ok();
    }

    [Authorize(Policy = "AdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {   
        if (string.IsNullOrEmpty(roles)) return BadRequest("Roles cannot be empty");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound("User not found");

        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }


    [Authorize(Policy = "AdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "ModeratorRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}
