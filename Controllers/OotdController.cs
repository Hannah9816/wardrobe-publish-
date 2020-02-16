using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WardRobe.Data;

namespace WardRobe.Controllers
{
    [Route("ootd")]
    public class OotdController : Controller
    {
        private ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;

        public OotdController(ApplicationDbContext _db, UserManager<IdentityUser> userManager)
        {
            db = _db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {

            return View();
        }

        [Route("findall")]
        public IActionResult FindAllEvents()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

        
            var events = db.Calendar.Where (e => e.UserId== userid).Select(e => new
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