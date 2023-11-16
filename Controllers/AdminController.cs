using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkerHours.Data;

namespace WorkerHours.Controllers
{
    public class AdminController : Controller
    {
        private readonly WorkerHoursContext _context;

        public AdminController(WorkerHoursContext context)
        {
            _context = context;
        }
        // GET: AdminController
        public ActionResult Index()
        {
            return View("Index");
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Report()
        {
            return View();
        }

        //gets worker details and the month to show the salary
        public async Task<IActionResult> ReportWorker(string firstName,string lastName,string selectedDate)
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
            (!DateTime.Equals(shift.EnteryTime, default(DateTime)))&&(shift.EnteryTime.Year== selected.Year) &&(shift.EnteryTime.Month == selected.Month));//takes only the shfts that have enterce and exit-for the given month and year
            var amountOfHours = shiftToCaluclate.Sum(shift => (shift.ExitTime - shift.EnteryTime).TotalHours);//total hour
            var salary = amountOfHours * worker.PricePerHour;
            return View(salary);
        }

    }
}
