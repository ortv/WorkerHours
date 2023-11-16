using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using WorkerHours.Data;
using WorkerHours.Models;

namespace WorkerHours.Controllers
{
    public class WorkersController : Controller
    {
        private readonly WorkerHoursContext _context;

        public WorkersController(WorkerHoursContext context)
        {
            _context = context;
        }

        // GET: Workers
        public async Task<IActionResult> Index()
        {
              return _context.Worker != null ? 
                          View(await _context.Worker.ToListAsync()) :
                          Problem("Entity set 'WorkerHoursContext.Worker'  is null.");
        }

        public async Task<IActionResult> WorkerDetails(string firstName, string lastName)
        {
           
            if (string.IsNullOrEmpty(firstName) ||string.IsNullOrEmpty(lastName))
            {
                return BadRequest("שגיאה");
            }
            var worker = _context.Worker
            .Include(w => w.Shifts)
            .FirstOrDefault(w => (w.FirstName == firstName) &&(w.LastName== lastName));//find the worker in the db

            if (worker == null)//worker is not found
            {
                return BadRequest("עובד לא קיים.");
            }

            return View( worker);
            
        }

        public async Task<IActionResult> RecordEntery(int workerId)//saves the enterce of this(id) worker
        {
            var worker = _context.Worker
            .Include(w => w.Shifts).FirstOrDefault(w=>w.Id==workerId);
            if(worker==null)
            {
                return BadRequest("עובד לא קיים.");
            }
            var shift = worker.Shifts.LastOrDefault();
            //if (shift.DayDate == DateTime.Today)//if already entered a shift today
            //{

            //}
            worker.Shifts.Add(new Shift { EnteryTime = DateTime.Now, DayDate = DateTime.Today });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> RecordExit(int workerId)//record exit time for worker
        {
            var worker = _context.Worker
            .Include(w => w.Shifts).FirstOrDefault(w => w.Id == workerId);
            if (worker == null)
            {
                return BadRequest("עובד לא קיים.");
            }
            var shift = worker.Shifts.LastOrDefault();
            if(shift==null|| !DateTime.Equals(shift.ExitTime, default(DateTime)))//has already exit or doesnt have a shift-error
            {
                return BadRequest("No active shift to exit from.");
            }

            shift.ExitTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Worker == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkerId,FirstName,LastName,PricePerHour")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(worker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        // GET: Workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Worker == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkerId,FirstName,LastName,PricePerHour")] Worker worker)
        {
            if (id != worker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.Id))
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
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            return View(worker);
        }

        // GET: Workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Worker == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Worker == null)
            {
                return Problem("Entity set 'WorkerHoursContext.Worker'  is null.");
            }
            var worker = await _context.Worker.FindAsync(id);
            if (worker != null)
            {
                _context.Worker.Remove(worker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id)
        {
          return (_context.Worker?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
