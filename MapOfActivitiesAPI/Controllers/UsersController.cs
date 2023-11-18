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
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly MapOfActivitiesAPIContext _context;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, MapOfActivitiesAPIContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet]
    [Route("with-roles")]
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
    [Route("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }
        var role = await _userManager.GetRolesAsync(user);

        var userData = new
        {
            user.Id,
            user.Email,
            Role = role
        };

        return Ok(userData);
    }

    [HttpPost]
    [Route("ban/{userId}")]
    public async Task<IActionResult> Ban(string userId)
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
    [Route("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var userProfile = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (userProfile != null)
        {
            _context.Users.Remove(userProfile);
            await _context.SaveChangesAsync();
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Ok(new { Message = "User deleted successfully." });
        }

        return BadRequest(new { Message = "User deletion failed." });
    }

    [HttpGet]
    [Route("profiles/{userId}")]
    public async Task<ActionResult<User>> GetUserProfile(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPut]
    [Route("profiles/{userId}")]
    public async Task<IActionResult> UpdateUserProfile(string userId, User user)
    {
        if (!string.Equals(userId, user.UserId))
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(userId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    //[HttpPost]
    //[Route("profiles")]
    //public async Task<ActionResult<User>> CreateUserProfile(User user)
    //{
    //    _context.Users.Add(user);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction(nameof(GetUserProfile), new { userId = user.UserId }, user);
    //}

    [HttpGet]
    [Route("roles")]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return Ok(roles);
    }

    [HttpGet]
    [Route("profiles")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUserProfiles()
    {
        return await _context.Users.ToListAsync();
    }

    private bool UserExists(string userId)
    {
        return _context.Users.Any(e => e.UserId == userId);
    }
}
