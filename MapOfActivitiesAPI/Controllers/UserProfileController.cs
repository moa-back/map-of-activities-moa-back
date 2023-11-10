using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MapOfActivitiesAPI.Models;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly MapOfActivitiesAPIContext _context;

    public UserProfileController(MapOfActivitiesAPIContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetUserProfile(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserProfile(int userId, User user)
    {
        if (userId != user.Id)
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

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserProfile(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int userId)
    {
        return _context.Users.Any(e => e.Id == userId);
    }
}
