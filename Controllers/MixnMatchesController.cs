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

namespace WardRobe.Views.MixnMatches
{
    public class MixnMatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MixnMatchesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MixnMatches
        public async Task<IActionResult> Index()
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);

            var userid = _userManager.GetUserId(HttpContext.User);

            var mixnmatch = from m in _context.MixnMatch select m;

            if (!String.IsNullOrEmpty(userid))
            {
                mixnmatch = mixnmatch.Where(m => m.UserId.Contains(userid));
            }
            return View(await mixnmatch.AsNoTracking().ToListAsync());
        }

        // GET: MixnMatches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mixnMatch = await _context.MixnMatch
                .FirstOrDefaultAsync(m => m.ID == id);
            if (mixnMatch == null)
            {
                return NotFound();
            }

            return View(mixnMatch);
        }

        // GET: MixnMatches/Create
        public IActionResult Create()
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);

            var userid = _userManager.GetUserId(HttpContext.User);

            var wardrobe = from m in _context.Wardrobe select m;

            ViewData["wardrobe"] = wardrobe.Where(m => m.UserId.Contains(userid)).ToList();

            return View();
        }

        // POST: MixnMatches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Top,Bottom,UserId")] MixnMatch mixnMatch)
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid)
            {
                _context.Add(mixnMatch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mixnMatch);
        }

        // GET: MixnMatches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mixnMatch = await _context.MixnMatch.FindAsync(id);
            if (mixnMatch == null)
            {
                return NotFound();
            }
            var userid = _userManager.GetUserId(HttpContext.User);

            var wardrobe = from m in _context.Wardrobe select m;

            ViewData["wardrobe"] = wardrobe.Where(m => m.UserId.Contains(userid)).ToList();

            return View(mixnMatch);
        }

        // POST: MixnMatches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Top,Bottom,UserId")] MixnMatch mixnMatch)
        {
            if (id != mixnMatch.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mixnMatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MixnMatchExists(mixnMatch.ID))
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
            return View(mixnMatch);
        }

        // GET: MixnMatches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mixnMatch = await _context.MixnMatch
                .FirstOrDefaultAsync(m => m.ID == id);
            if (mixnMatch == null)
            {
                return NotFound();
            }

            return View(mixnMatch);
        }

        // POST: MixnMatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mixnMatch = await _context.MixnMatch.FindAsync(id);
            _context.MixnMatch.Remove(mixnMatch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MixnMatchExists(int id)
        {
            return _context.MixnMatch.Any(e => e.ID == id);
        }
    }
}
