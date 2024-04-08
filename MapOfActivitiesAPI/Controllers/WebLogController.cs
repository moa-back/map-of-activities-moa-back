using MapOfActivitiesAPI.Interfaces;
using MapOfActivitiesAPI.Models;
using MapOfActivitiesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapOfActivitiesAPI.Controllers
{
    public class WebLogController : Controller
    {
        private MapOfActivitiesAPIContext _context;

        public WebLogController(MapOfActivitiesAPIContext context)
        {
            _context = context;
        }

        [Authorize(Roles = ApplicationUserRoles.Admin)]
        [HttpGet]
        public IEnumerable<WebLog> Get()
        {
            return _context.WebLogs;
        }
    }
}
