using MapOfActivitiesAPI.Models;
using MapOfActivitiesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.SignalR;
using MapOfActivitiesAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;

namespace MapOfActivitiesAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentsController : ControllerBase
	{
		private readonly INotificationHub _hubContext;
		private MapOfActivitiesAPIContext _context;
		public CommentsController(INotificationHub hubContext, MapOfActivitiesAPIContext context)
		{
			_hubContext = hubContext;
			_context = context;

		}

		[HttpGet]
		public IEnumerable<Comment> Get()
		{
			return _context.Comments.ToList();
		}

        [HttpPost]
        [Route("delete-comments")]
        public async Task<IActionResult> DeleteComments(int eventId)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }
            var comments = await _context.Comments.Where(v => v.EventId == eventId).ToListAsync();
            if (comments == null)
            {
                return NotFound();
            }
            for (int i = 0; i < comments.Count; i++)
            {
                _context.Comments.Remove(comments[i]);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}
