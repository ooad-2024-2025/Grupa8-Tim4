using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cvjecara_Latica.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrator"))
            {
                var allReports = _context.Reports.Include(r => r.Person);
                return View(await allReports.ToListAsync());
            }
            else
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var myReports = _context.Reports
                    .Where(r => r.PersonID == userId)
                    .Include(r => r.Person);
                return View(await myReports.ToListAsync());
            }
        }

        // GET: Reports/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var report = await _context.Reports
                .Include(r => r.Person)
                .FirstOrDefaultAsync(m => m.ReportID == id);

            if (report == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Administrator") || report.PersonID == userId)
            {
                return View(report);
            }

            return Forbid(); 
        }


        // GET: Reports/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            var users = _context.Users.ToList();
            ViewData["PersonID"] = new SelectList(users, "Id", "Id");
            ViewBag.PersonList = users.ToDictionary(u => u.Id, u => u.Email); // ili Ime + Prezime

            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportID,ReportType,Date,PersonID,Content")] Report report)
        {
            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var users = _context.Users.ToList();
            ViewData["PersonID"] = new SelectList(users, "Id", "Id", report.PersonID);
            ViewBag.PersonList = users.ToDictionary(u => u.Id, u => u.Email);

            return View(report);
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            var users = _context.Users.ToList();
            ViewData["PersonID"] = new SelectList(users, "Id", "Id", report.PersonID);
            ViewBag.PersonList = users.ToDictionary(u => u.Id, u => u.Email);

            return View(report);
        }
        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportID,ReportType,Date,PersonID,Content")] Report report)
        {
            if (id != report.ReportID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.ReportID))
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
            
            return View(report);
        }

        // GET: Reports/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .Include(r => r.Person)
                .FirstOrDefaultAsync(m => m.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Reports/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.ReportID == id);
        }
    }
}
