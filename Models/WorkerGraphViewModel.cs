namespace WorkerHours.Models
{//model for generating graph. for list of hours there is the matching list of salary.
    public class WorkerGraphViewModel
    {
        public List<double> HoursForTheDay { get; set; }
        public List<double> SalaryForTheDay { get; set; }
        // Add properties for axis labels
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
    }
}
