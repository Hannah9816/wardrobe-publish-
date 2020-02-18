using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WardRobe.Data;
using WardRobe.Models;

namespace WardRobe.Views.Backpacks
{
    public class BackpacksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BackpacksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IList<Backpack> Backpack { get; set; }

        // GET: Backpacks
        public async Task<IActionResult> Index(string Tripname)
        {
            var trip = from t in _context.Backpack select t;

            IQueryable<string> TypeQuery = from t in _context.Backpack
                                           orderby t.TripName
                                           select t.TripName;

            IEnumerable<SelectListItem> items =
                new SelectList(await TypeQuery.Distinct().ToListAsync());

            ViewBag.userid = _userManager.GetUserId(HttpContext.User);

            var userid = _userManager.GetUserId(HttpContext.User);

            ViewBag.Tripname = items;

            if (!String.IsNullOrEmpty(Tripname))
            {
                trip = trip.Where(t => t.TripName == Tripname && t.UserId.Contains(userid));
            }

            if (!String.IsNullOrEmpty(userid))
            {
                trip = trip.Where(t => t.UserId.Contains(userid));
            }

            return View(await trip.AsNoTracking().ToListAsync());
        }

        // GET: Backpacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backpack = await _context.Backpack
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backpack == null)
            {
                return NotFound();
            }

            return View(backpack);
        }

        // GET: Backpacks/Create
        public IActionResult Create()
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);

            var userid = _userManager.GetUserId(HttpContext.User);

            var wardrobe = from m in _context.Wardrobe select m;

            var trip = from t in _context.Trip select t;

            ViewData["wardrobe"] = wardrobe.Where(m => m.UserId.Contains(userid)).ToList();

            ViewData["trip"] = trip.Where(t => t.UserId.Contains(userid)).ToList();

            return View();
        }

        // POST: Backpacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TripName,Wardrobe,UserId")] Backpack backpack)
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);

            if (ModelState.IsValid)
            {
                _context.Add(backpack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(backpack);
        }

        // GET: Backpacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backpack = await _context.Backpack.FindAsync(id);
            if (backpack == null)
            {
                return NotFound();
            }
            var userid = _userManager.GetUserId(HttpContext.User);

            var wardrobe = from m in _context.Wardrobe select m;

            var trip = from t in _context.Trip select t;

            ViewData["wardrobe"] = wardrobe.Where(m => m.UserId.Contains(userid)).ToList();

            ViewData["trip"] = trip.Where(t => t.UserId.Contains(userid)).ToList();

            return View(backpack);
        }

        // POST: Backpacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TripName,Wardrobe,UserId")] Backpack backpack)
        {
            if (id != backpack.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(backpack);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BackpackExists(backpack.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(backpack);
        }

        // GET: Backpacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backpack = await _context.Backpack
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backpack == null)
            {
                return NotFound();
            }

            return View(backpack);
        }

        // POST: Backpacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var backpack = await _context.Backpack.FindAsync(id);
            _context.Backpack.Remove(backpack);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BackpackExists(int id)
        {
            return _context.Backpack.Any(e => e.Id == id);
        }

    }
}
