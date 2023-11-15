using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WorkerHours.Models
{
    public class Worker
    {
        public int Id { get; set; }

        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }
        [Display(Name = "שם משפחה")]
        public string LastName { get; set; }
        [Display(Name = "מחיר לשעה")]
        public double PricePerHour { get; set; }
        [Display(Name = "משמרות")]
        public List<Shift>? Shifts { get; set; }


    }
}
