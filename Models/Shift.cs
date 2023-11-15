using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WorkerHours.Models
{
    public class Shift
    {
        public int Id { get; set; }
        [Display(Name = "שעת כניסה")]
        public DateTime EnteryTime { get; set; }
        [Display(Name = "שעת יציאה")]
        public DateTime ExitTime { get; set; }
        [Display(Name = "תאריך")]
        public DateTime DayDate { get; set; }

    }
}
