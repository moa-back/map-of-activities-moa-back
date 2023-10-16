using MapOfActivitiesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Type = MapOfActivitiesAPI.Models.Type;

namespace MapOfActivitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeController : ControllerBase
    {
        private MapOfActivitiesAPIContext _context;

        public TypeController(MapOfActivitiesAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Type> Get()
        {
            return _context.Types.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Type> Get(int id)
        {
            var type = _context.Types.FirstOrDefault(x => x.Id == id);

            if (type == null)
            {
                return NotFound();
            }

            return type;
        }

        [HttpPost]
        public ActionResult<Type> Post([FromBody] Type type)
        {
            if (type == null)
            {
                return BadRequest("Type object is null");
            }

            _context.Types.Add(type);
            _context.SaveChanges();

            return CreatedAtAction("Get", new { id = type.Id }, type);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Type type)
        {
            if (id != type.Id)
            {
                return BadRequest("Type ID does not match");
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Types.Any(e => e.Id == id))
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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var type = _context.Types.Find(id);

            if (type == null)
            {
                return NotFound();
            }

            _context.Types.Remove(type);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
