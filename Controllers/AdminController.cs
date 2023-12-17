using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkerHours.Data;
using WorkerHours.Models;

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
        public ActionResult GraphContent()
        {
            return View();
        }
        //create a graph that shows, according to a spesific worker, the hours of each day in a month
        public IActionResult WorkerGraph(string workerId,string month)
        {//month parameter is in __/____ pattern
            string year1 = month.Substring(3, 4);
            int.TryParse(year1, out int year);
            string month1 = month.Substring(0, 2);
            int.TryParse(month1, out int month2);

            var worker = _context.Worker
             .Include(w => w.Shifts)
             .FirstOrDefault(w => w.WorkerId == workerId);

            if (worker == null)
            {
                return NotFound();
            }
            //found the worker

            //takes only thr shifts that are full-have entry and exit
            var shiftsInMonth = worker.Shifts
            .Where(shift => (!DateTime.Equals(shift.EnteryTime, default(DateTime)) && !DateTime.Equals(shift.ExitTime, default(DateTime))) &&(shift.EnteryTime.Year == year && shift.EnteryTime.Month == month2))
            .ToList();
            var hours = new List<double>();
            var salaries = new List<double>();
            foreach (var shift in shiftsInMonth)
            {
                hours.Add((shift.ExitTime - shift.EnteryTime).TotalHours);//add to the list the amount of hours
                salaries.Add(((shift.ExitTime - shift.EnteryTime).TotalHours) * worker.PricePerHour);//add the salary for this amount of hours
            }
            var viewModel = new WorkerGraphViewModel
            {
                HoursForTheDay = hours,
                SalaryForTheDay = salaries,
                XAxisLabel = "Amount of Hours",
                YAxisLabel = "Price to Pay"
            };

            ViewBag.WorkerName = $"{worker.FirstName} {worker.LastName}";
            ViewBag.Month = month2;
            ViewBag.Year = year1;

            return View(viewModel);
        }
        //manual shift for worker
        public IActionResult ManualShift(string id, DateTime date, DateTime timeEnter, DateTime timeExit)
        {//get id of worker,date,time enter, time exit and creates salary
            //if updated succsesfully-will be a massege
            var worker = _context.Worker
             .Include(w => w.Shifts)
             .FirstOrDefault(w => w.WorkerId == id);

            if (id!=null&&worker == null)//worker not found
            {
                ViewBag.Message = "Error:worker doesnt exist";
                return View(); 
            }
            if (id==null)//dont hav input yet
            {
                return View();
            }
            Shift shift=new Shift { DayDate=date,EnteryTime=timeEnter,ExitTime=timeExit};//creates a shift
            worker.Shifts.Add(shift);
            ViewBag.Message = "Shift added successfully.";

            return View();
        }
        //shows a  list of all the workers right now
        public IActionResult StatusWorkers()
        {
            var actualTime = DateTime.Now;//gets the actual time and date
                                          //takes only the worker that there last shift is from today and they havnt exit yet
            var wo = _context.Worker.Include(w => w.Shifts)
                .Include(w => w.Salaries).ToList();
            var working=wo.Where(w => w.Shifts.Any() && DateTime.Equals(w.Shifts.Last().DayDate, actualTime.Date) &&
                DateTime.Equals(w.Shifts.Last().ExitTime, default(DateTime))).ToList();
            //var working = _context.Worker
            //    .Include(w => w.Shifts)
            //    .Include(w => w.Salaries)
            //    .Where(w => w.Shifts.Any() &&
            //    DateTime.Equals(w.Shifts.OrderByDescending(s => s.DayDate).First().DayDate, actualTime.Date) &&
            //    DateTime.Equals(w.Shifts.OrderByDescending(s => s.DayDate).First().ExitTime, default(DateTime))).ToList();

            return View("~/Views/Workers/showWorkers.cshtml", working);
        }
        //if the worker forot to submit an exit- the admin has the option to fix it
        public IActionResult ForgotExit(string id,DateTime exit)
        {
            var worker = _context.Worker
             .Include(w => w.Shifts)
             .FirstOrDefault(w => w.WorkerId == id);

            if (id != null && worker == null)//worker not found
            {
                ViewBag.Message = "Error:worker doesnt exist";
                return View();
            }
            if (id == null)//dont have input yet
            {
                return View();
            }
            if (worker.Shifts.Count()==0||!DateTime.Equals(worker.Shifts.Last().ExitTime, default(DateTime)))//there is already an exit or doesnt have shifts a all(not even an enterce)
            {
                ViewBag.Message = "Error:exit already exists";
                return View();
            }
            worker.Shifts.Last().ExitTime = exit;//updates the lat shift(we know that he did an enterce) to the actual exit time
            _context.SaveChanges();
            ViewBag.Message = "exit updated successfully.";
            return View();

        }

    }
}
