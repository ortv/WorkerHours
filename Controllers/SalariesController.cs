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
    public class SalariesController : Controller
    {
        private readonly WorkerHoursContext _context;

        public SalariesController(WorkerHoursContext context)
        {
            _context = context;
        }

        // GET: Salaries
        public async Task<IActionResult> Index()
        {
              return _context.Salary != null ? 
                          View(await _context.Salary.ToListAsync()) :
                          Problem("Entity set 'WorkerHoursContext.Salary'  is null.");
        }

        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Salary == null)
            {
                return NotFound();
            }

            var salary = await _context.Salary
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Salaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Month,Year,TotalOfHours,SalaryOfMonth")] Salary salary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salary);
        }

        // GET: Salaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Salary == null)
            {
                return NotFound();
            }

            var salary = await _context.Salary.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }
            return View(salary);
        }

        // POST: Salaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Month,Year,TotalOfHours,SalaryOfMonth")] Salary salary)
        {
            if (id != salary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryExists(salary.Id))
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
            return View(salary);
        }

        // GET: Salaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Salary == null)
            {
                return NotFound();
            }

            var salary = await _context.Salary
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Salary == null)
            {
                return Problem("Entity set 'WorkerHoursContext.Salary'  is null.");
            }
            var salary = await _context.Salary.FindAsync(id);
            if (salary != null)
            {
                _context.Salary.Remove(salary);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryExists(int id)
        {
          return (_context.Salary?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        //gets worker details and the month to show the salary
        public async Task<IActionResult> ReportWorker(string firstName, string lastName, string selectedDate)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return BadRequest("שגיאה");
            }
            DateTime selected = DateTime.Parse(selectedDate);

            var worker = _context.Worker
            .Include(w => w.Shifts)
            .FirstOrDefault(w => (w.FirstName == firstName) && (w.LastName == lastName));//find the worker in the db

            if (worker == null)//worker is not found
            {
                return BadRequest("עובד לא קיים.");
            }

            //need to calculate sum of hours in the month and total salary
            var shiftToCaluclate = worker.Shifts.Where(shift => (!DateTime.Equals(shift.ExitTime, default(DateTime))) &&
            (!DateTime.Equals(shift.EnteryTime, default(DateTime))) && (shift.EnteryTime.Year == selected.Year) && (shift.EnteryTime.Month == selected.Month));//takes only the shfts that have enterce and exit-for the given month and year
            var amountOfHours = shiftToCaluclate.Sum(shift => (shift.ExitTime - shift.EnteryTime).TotalHours);//total hour
            var money = amountOfHours * worker.PricePerHour;
            // Ensure Salaries property is not null
            if (worker.Salaries == null)
            {
                worker.Salaries = new List<Salary>();
            }
            else//need to make sure that this month wasnt alredy added to list of salaries
            {
                var check= worker.Salaries.FirstOrDefault(s=>s.Year== selected.Year&&s.Month== selected.Month);
                if(check!=null)//there is a report for this salary already
                {
                    return RedirectToAction("Details", new { id = check.Id });//report about this one

                }
            }
            //else-its a new report
            Salary salary = new Salary { Month=selected.Month,Year=selected.Year ,TotalOfHours=amountOfHours,SalaryOfMonth=money};//create an object of salary
            worker.Salaries.Add(salary);//add salary to salaries list of worker            
            await _context.SaveChangesAsync();// Save changes to the database
            return RedirectToAction("Details", new { id = salary.Id });
        }
    }
}
