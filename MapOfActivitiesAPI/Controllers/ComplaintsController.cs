using MapOfActivitiesAPI.Interfaces;
using MapOfActivitiesAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MapOfActivitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : Controller
    {
        private MapOfActivitiesAPIContext _context;

        public ComplaintsController(MapOfActivitiesAPIContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        [Route("all-events-complaints")]
        public async Task<IActionResult> GetEventsComplaints()
        {
            if (_context.Complaints == null)
            {
                return NotFound();
            }

            var complaints = await _context.Complaints
                .Where(c => c.EventId != null)
                .Select(c => new
                {
                    c.EventId,
                    c.Header,
                    c.Description,
                    c.AuthorId,
                    AuthorEmail = _context.Users.FirstOrDefault(u => u.Id == c.AuthorId) != null ? _context.Users.FirstOrDefault(u => u.Id == c.AuthorId).Email : null,
                })
                .ToListAsync();

            if (complaints == null)
            {
                return NoContent();
            }

            return Ok(complaints);
        }

        
        [HttpGet]
        [Route("all-users-complaints")]
        public async Task<IActionResult> GetUsersComplaints()
        {

            var complaints = await _context.Complaints
                .Where(c => c.UserId != null)
                .Select(c => new
                {
                    c.Id,
                    c.Header,
                    c.Description,
                    UserId = _context.Users.FirstOrDefault(u => u.Id == c.UserId) != null ? _context.Users.FirstOrDefault(u => u.Id == c.UserId).UserId : null,
                    UserEmail = _context.Users.FirstOrDefault(u => u.Id == c.UserId) != null ? _context.Users.FirstOrDefault(u => u.Id == c.UserId).Email : null,
                    AuthorEmail = _context.Users.FirstOrDefault(u => u.Id == c.AuthorId) != null ? _context.Users.FirstOrDefault(u => u.Id == c.AuthorId).Email : null,
                })
                .ToListAsync();

            if (complaints == null)
            {
                return NoContent();
            }

            return Ok(complaints);
        }



        /*
        [HttpGet]
        [Route("all-users-complaints")]
        public async Task<IActionResult> GetUsersComplaints()
        {
            if (_context.Complaints == null)
            {
                return NotFound();
            }

            var complaints = await _context.Complaints.Where(c => c.UserId != null).ToListAsync();

            //var userIds = complaints.Select(c => c.UserId).Distinct().ToList();


            if (complaints == null)
            {
                return NoContent();
            }

            return Ok(complaints); // треба додати пошту користувача й автора скарги у відправку на фронтенд
        }
        */

        [HttpGet]
        [Route("user-complaints/{userId}")]
        public async Task<IActionResult> GetUserComplaints(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var userComplaints = await _context.Complaints
               .Where(c => c.UserId == user.Id)
               .Select(c => new
               {
                   c.Id,
                   c.Header,
                   c.Description,
                   AuthorEmail = _context.Users.FirstOrDefault(u => u.Id == c.AuthorId) != null ? _context.Users.FirstOrDefault(u => u.Id == c.AuthorId).Email : null,
               })
               .ToListAsync();


            if (userComplaints == null)
            {
                return NotFound();
            }

            return Ok(userComplaints);
        }


        //[Authorize(Roles = ApplicationUserRoles.Admin)]
        [HttpPost]
        [Route("user-complaint")]
        public async Task<ActionResult<Complaint>> PostComplaintUser(string header, string description, string userId, string userIdAuthor)
        {

            var toUser = await _context.Users.FirstOrDefaultAsync(e => e.UserId == userId);

            if (toUser == null)
            {
                return NotFound();
            }

            var fromUser = await _context.Users.FirstOrDefaultAsync(e => e.UserId == userIdAuthor);

            if (fromUser == null)
            {
                return NotFound();
            }

            Complaint myUserComplaint = new Complaint();
            myUserComplaint.Header = header;
            myUserComplaint.Description = description;
            myUserComplaint.UserId = toUser.Id;
            myUserComplaint.AuthorId = fromUser.Id;

            _context.Complaints.Add(myUserComplaint);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //[Authorize(Roles = ApplicationUserRoles.Admin)]
        [HttpDelete("user-complaint/{complaintId}")]
        public async Task<ActionResult<Complaint>> DeleteUserComplaint(string userId, int complaintId)
        {
            if (_context.Complaints == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserId == userId);

            var complaint = await _context.Complaints.Where(c => c.UserId == user.Id && c.Id == complaintId).FirstOrDefaultAsync();

            if (complaint == null)
            {
                return NotFound();
            }

            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //[Authorize(Roles = ApplicationUserRoles.Admin)]
        [HttpPost]
        [Route("event-complaint")]
        public async Task<ActionResult<Complaint>> PostComplainEvent(Complaint c)
        {

            var _event = await _context.Events.FirstOrDefaultAsync(e => e.Id == c.EventId);

            if (_event == null)
            {
                return NotFound();
            }

            //var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == c.UserId);

            //if (author == null)
            //{
            //    return NotFound();
            //}

            Complaint myUserComplaint = new Complaint();
            myUserComplaint.Header = c.Header;
            myUserComplaint.Description = c.Description;
            myUserComplaint.EventId = _event.Id;
            myUserComplaint.AuthorId = c.AuthorId;

            _context.Complaints.Add(myUserComplaint);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //[Authorize(Roles = ApplicationUserRoles.Admin)]
        [HttpDelete("event-complaint/{complaintId}")]
        public async Task<ActionResult<Complaint>> DeleteEventComplaint(int eventId, int complaintId)
        {
            if (_context.Complaints == null)
            {
                return NotFound();
            }

            var _event = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);

            var complaint = await _context.Complaints.Where(c => c.EventId == _event.Id && c.Id == complaintId).FirstOrDefaultAsync();

            if (complaint == null)
            {
                return NotFound();
            }

            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
