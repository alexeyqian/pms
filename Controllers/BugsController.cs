using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using PMS.Data;
using PMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Controllers
{
    public class BugsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly PMSDBContext _context;

        private static SelectList _statusList = new SelectList(new List<string> { "FixedAndApproved", "FixedAndWaitingForApproval",
                    "FixedWithoutCodeChange", "WorkedAndContinue", "WorkedButNotABug", "WorkedButOutOfScope" });
        private static SelectList _devList = new SelectList(new List<string> { "Kai Zhou", "Ling Nan Yang", "Qiang Qiang Guo", "Wei Yu" });

        public BugsController(IConfiguration configuration, PMSDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Bug.OrderByDescending(r=>r.Id).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bug
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bug == null)
            {
                return NotFound();
            }

            return View(bug);
        }

        public IActionResult Create()
        {
            ViewBag.StatusList = _statusList;
            ViewBag.DevList = _devList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NO,Status,Developer,StartedDate,FixedDate,EstimatedHours,ActualHours,Note")] Bug bug)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bug);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bug.FindAsync(id);
            if (bug == null)
            {
                return NotFound();
            }

            var selectedStatus = _statusList.Where(x => x.Text == bug.Status).First();
            selectedStatus.Selected = true;
            var selectedDeveloper = _devList.Where(x => x.Text == bug.Developer).First();
            selectedDeveloper.Selected = true;

            ViewBag.StatusList = _statusList;
            ViewBag.DevList = _devList;
            return View(bug);
        }
           
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Developer,StartedDate,FixedDate,ActualHours,Note")] Bug bug)
        {
            if (id != bug.Id)
            {
                return NotFound();
            }

            var existing = await _context.Bug.FindAsync(bug.Id);
            if (existing == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existing.Status = bug.Status;
                    existing.Developer = bug.Developer;
                    existing.StartedDate = bug.StartedDate;
                    existing.FixedDate = bug.FixedDate;
                    existing.ActualHours = bug.ActualHours;
                    existing.Note = bug.Note;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BugExists(bug.Id))
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
            return View(bug);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bug
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bug == null)
            {
                return NotFound();
            }

            return View(bug);
        }

        /*
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bug = await _context.Bug.FindAsync(id);
            _context.Bug.Remove(bug);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/
               
        public async Task<IActionResult> Sync(int id)
        {
            int no = id;
            if (no <= 0)
            {
                return NotFound();
            }

            try
            {
                var syncHelper = new SyncHelper(_configuration, _context);
                await syncHelper.SyncBug(no);
            }
            catch 
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public static int GetPullRequestAge(Bug bug)
        {
            if (!bug.FirstPullRequestDate.HasValue) return 0;

            var endDate = DateTime.Now;
            if (bug.ResovedDate.HasValue)
                endDate = bug.ResovedDate.Value;

            return endDate.Subtract(bug.FirstPullRequestDate.Value).Days + 1;
        }

        private bool BugExists(int id)
        {
            return _context.Bug.Any(e => e.Id == id);
        }
    }
}
