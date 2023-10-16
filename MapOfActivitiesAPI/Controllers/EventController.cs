using MapOfActivitiesAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace MapOfActivitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private MapOfActivitiesAPIContext _context;
        public EventController(MapOfActivitiesAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Event> Get()
        {
            return _context.Events;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            if (_context.Events == null)
            {
                return NotFound();
            }
            var confectioner = await _context.Events.FindAsync(id);

            if (confectioner == null)
            {
                return NotFound();
            }

            return confectioner;
        }

        [HttpGet("{filter}")]
        public IEnumerable<Event> GetEventsByFilter(string filter)
        {
            var filterType = _context.Types.Where(x => x.Name == filter).Select(x=>x.Id).FirstOrDefault();
            var points = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                points = points.Where(p => p.TypeId == filterType);
            }

            return (IEnumerable<Event>)points.ToList();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, Event myEvent)
        {
            if (id != myEvent.Id)
            {
                return BadRequest();
            }
            else{ }
            _context.Entry(myEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if ((_context.Events?.Any(e => e.Id == id)).GetValueOrDefault())
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

        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(Event myEvent)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'MapOfActivitiesAPIContext.Events'  is null.");
            }
            _context.Events.Add(myEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (_context.Events == null)
            {
                return NotFound();
            }
            var myEvent = await _context.Events.FindAsync(id);
            if (myEvent == null)
            {
                return NotFound();
            }
            _context.Events.Remove(myEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
