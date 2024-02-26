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

            var complaints = await _context.Complaints.Where(c => c.EventId != null).ToListAsync();

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
            if (_context.Complaints == null)
            {
                return NotFound();
            }

            var complaints = await _context.Complaints.Where(c => c.UserId != null).ToListAsync();

            if (complaints == null)
            {
                return NoContent();
            }

            return Ok(complaints);
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
        public async Task<ActionResult<Complaint>> PostComplainEvent(string header, string description, int eventId, string userIdAuthor)
        {

            var _event = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);

            if (_event == null)
            {
                return NotFound();
            }

            var author = await _context.Users.FirstOrDefaultAsync(e => e.UserId == userIdAuthor);

            if (author == null)
            {
                return NotFound();
            }

            Complaint myUserComplaint = new Complaint();
            myUserComplaint.Header = header;
            myUserComplaint.Description = description;
            myUserComplaint.EventId = _event.Id;
            myUserComplaint.AuthorId = author.Id;

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
