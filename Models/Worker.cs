using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WorkerHours.Models
{
    public class Worker
    {
        public int Id { get; set; }
        [Display(Name = "תעודת זהות")]

        public string WorkerId { get; set; }

        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }
        [Display(Name = "שם משפחה")]
        public string LastName { get; set; }
        [Display(Name = "מחיר לשעה")]
        public double PricePerHour { get; set; }
        [Display(Name = "משמרות")]
        public List<Shift>? Shifts { get; set; }//all the shifts of worker over time
        public List<Salary>? Salaries { get; set; }//saves all the salries of worker ever the month

    }
}
