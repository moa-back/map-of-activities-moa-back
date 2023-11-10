using MapOfActivitiesAPI.Interfaces;
using MapOfActivitiesAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace MapOfActivitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;
        private MapOfActivitiesAPIContext _context;
        public EventController(IFileStorage fileStorage, MapOfActivitiesAPIContext context)
        {
            _fileStorage = fileStorage;
            _context = context;
        }
     //   [Authorize(Roles = ApplicationUserRoles.User)]
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
            var confectioner = _context.Events.Include(x => x.Type).Where(x => x.Id == id).FirstOrDefault();

            if (confectioner == null)
            {
                return NotFound();
            }

            return confectioner;
        }

        //[HttpGet("{filter}")]
        //public IEnumerable<Event> GetEventsByFilter(string filter)
        //{
        //    var filterType = _context.Types.Where(x => x.Name == filter).Select(x=>x.Id).FirstOrDefault();
        //    var points = _context.Events.AsQueryable();

        //    if (!string.IsNullOrEmpty(filter))
        //    {
        //        points = points.Where(p => p.TypeId == filterType);
        //    }

        //    return (IEnumerable<Event>)points.ToList();
        //}


        [HttpGet("filter")]
        public IEnumerable<Event> GetEventsByFilter([FromQuery] string? searchName = null,
            [FromQuery] string? userPoint = null,
            [FromQuery] double? distance = null,
            [FromQuery] List<int>? types = null,
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null)
        {
            var points = _context.Events.Include(x => x.Type).AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                points = points.AsEnumerable().Where(p => p.Name.ToLower().Contains(searchName.ToLower())).AsQueryable();
            }

            if (!string.IsNullOrEmpty(userPoint) && distance.HasValue)
            {
                var allPoints = points.ToList();
                points = allPoints.Where(p => CalculateDistance(p.Coordinates, userPoint) <= distance.Value).AsQueryable();
            }

            if (types != null && types.Count > 0)
            {
                points = points.Where(p => types.Contains(p.TypeId));
            }

            if (startTime.HasValue)
            {
                points = points.Where(p => p.StartTime >= startTime.Value);
            }

            if (endTime.HasValue)
            {
                points = points.Where(p => p.EndTime <= endTime.Value);
            }

                return (IEnumerable<Event>)points.ToList();
            }

            private double CalculateDistance(string coordinates1, string coordinates2)
            {

                var (lat1, lon1) = ParseCoordinates(coordinates1);
                var (lat2, lon2) = ParseCoordinates(coordinates2);

                const double EarthRadius = 6371000;


                var lat1Rad = DegreeToRadian(lat1);
                var lon1Rad = DegreeToRadian(lon1);
                var lat2Rad = DegreeToRadian(lat2);
                var lon2Rad = DegreeToRadian(lon2);


                var dLat = lat2Rad - lat1Rad;
                var dLon = lon2Rad - lon1Rad;
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                var distance = EarthRadius * c;
                return distance;

            }
            private (double, double) ParseCoordinates(string coordinates)
            {

                coordinates = coordinates.Trim().Replace(" ", "");


                var parts = coordinates.Split(',');

                if (parts.Length == 2 &&
                    double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                {
                    return (lat, lon);
                }

                throw new ArgumentException("Invalid coordinates format");
            }


            private double DegreeToRadian(double degree)
            {
                return degree * Math.PI / 180.0;
            }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutEvent(int id, Event myEvent)
        //{
        //    if (id != myEvent.Id)
        //    {
        //        return BadRequest();
        //    }
        //    else { }

        //    _context.Entry(myEvent).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if ((_context.Events?.Any(e => e.Id == id)).GetValueOrDefault())
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, EventView viewEvent)
        {
            if (id != viewEvent.Id)
            {
                return BadRequest();
            }
            else { }

            var myEvent = await _context.Events.FindAsync(id);
            if(viewEvent.DataUrl != "")
            {
                _fileStorage.Delete(myEvent.ImageName);
                myEvent.ImageName = await _fileStorage.Upload(viewEvent.DataUrl);
            }
            myEvent.Name = viewEvent.Name;
            myEvent.TypeId = viewEvent.TypeId;
            myEvent.StartTime = viewEvent.StartTime;
            myEvent.EndTime = viewEvent.EndTime;
            myEvent.Description = viewEvent.Description;
            myEvent.Coordinates = viewEvent.Coordinates;
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
            public async Task<ActionResult<Event>> PostEvent(EventView viewEvent)
            {
                if (_context.Events == null)
                {
                    return Problem("Entity set 'MapOfActivitiesAPIContext.Events'  is null.");
                }
             
                Event myEvent = new Event();
                myEvent.ImageName = await _fileStorage.Upload(viewEvent.DataUrl);
                myEvent.Name = viewEvent.Name;
                myEvent.TypeId = viewEvent.TypeId;
                myEvent.StartTime = viewEvent.StartTime;
                myEvent.EndTime = viewEvent.EndTime;
                myEvent.Description = viewEvent.Description;
                myEvent.Coordinates = viewEvent.Coordinates;

                _context.Events.Add(myEvent);
                await _context.SaveChangesAsync();

                return NoContent();
            }

        //[HttpGet("{fileName}")]
        //public async Task<ActionResult<string>> GetImage(string fileName)
        //{
        //    return await _fileStorage.Get(fileName); 
        //}

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
                _fileStorage.Delete(myEvent.ImageName);
                _context.Events.Remove(myEvent);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
