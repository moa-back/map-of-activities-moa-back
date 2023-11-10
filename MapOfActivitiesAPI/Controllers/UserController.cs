using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MapOfActivitiesAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    [Route("users")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users.ToListAsync();

        var usersWithRoles = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            usersWithRoles.Add(new
            {
                user.Id,
                user.Email,
                Roles = roles
            });
        }

        return Ok(usersWithRoles);
    }


    [HttpGet]
    [Route("users/{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var userData = new
        {
            user.Id,
            user.Email
        };

        return Ok(userData);
    }

    [HttpPost]
    [Route("banned/{userId}")]
    public async Task<IActionResult> AddBanned(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        if (!await _roleManager.RoleExistsAsync(ApplicationUserRoles.BannedUser))
        {
            await _roleManager.CreateAsync(new IdentityRole(ApplicationUserRoles.BannedUser));
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
        }

        await _userManager.AddToRoleAsync(user, ApplicationUserRoles.BannedUser);

        return Ok(new { Message = "User banned successfully." });
    }


    [HttpDelete]
    [Route("delete-user/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Ok(new { Message = "User deleted successfully." });
        }

        return BadRequest(new { Message = "User deletion failed." });
    }

}
