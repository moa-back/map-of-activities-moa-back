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
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationHub _hubContext;
        private MapOfActivitiesAPIContext _context;
        public NotificationsController(INotificationHub hubContext, MapOfActivitiesAPIContext context)
        {
            _hubContext = hubContext;
            _context = context;

        }

        [HttpGet]
        public IEnumerable<Message> Get()
        {
            return _context.Messages.ToList();
        }

        [Authorize(Roles = ApplicationUserRoles.User + "," + ApplicationUserRoles.Admin)]
        [HttpPost]
        public async Task<ActionResult<Event>> PostMessage(string name, string msg)
        {
            if (msg == null)
            {
                return Problem("Msg is null.");
            }
            Message myMsg = new Message();
            myMsg.Name = name;
            myMsg.UserId = -1;
            myMsg.Text = msg;
            _context.Messages.Add(myMsg);
            await _context.SaveChangesAsync();
            await _hubContext.SendMessageToEveryone(name, msg);
            return NoContent();
        }
    }
}
