using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MapOfActivitiesAPI.Models;


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
    public async Task<ActionResult<User>> GetUserProfile(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPut("{userId}")]
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

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserProfile(string userId)
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

    [HttpPost]
    public async Task<ActionResult<User>> CreateUserProfile(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserProfile), new { userId = user.UserId }, user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUserProfiles()
    {
        return await _context.Users.ToListAsync();
    }

    private bool UserExists(string userId)
    {
        return _context.Users.Any(e => e.UserId == userId);
    }
}
