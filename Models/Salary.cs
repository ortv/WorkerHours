using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WorkerHours.Models
{
    public class Salary
    {//define a salary for worker in  a spesific month
        //
        public int Id { get; set; }

        [Display(Name = "חודש")]
        public int Month { get; set; }

        [Display(Name = "שנה")]
        public int Year { get; set; }

        [Display(Name = "סך שעות")]
        public double TotalOfHours { get; set; }

        [Display(Name = "סך הכל לתשלום")]
        public double SalaryOfMonth { get; set; }
    }
}
