using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WardRobe.Data;

namespace WardRobe.Controllers
{
    [Route("ootd")]
    public class OotdController : Controller
    {
        private ApplicationDbContext db;

        public OotdController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("findall")]
        public IActionResult FindAllEvents()
        {
            var events = db.Calendar.Select(e => new
            {
                id = e.Id,
                description = e.Description,
                start = e.StartDate.ToString(),
                end = e.EndDate.ToString(),
            }).ToList();
            return new JsonResult(events);
        }
    }
}