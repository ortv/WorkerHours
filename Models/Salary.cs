namespace WorkerHours.Models
{
    public class Salary
    {//define a salary for worker in  a spesific month
        //
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double TotalOfHours { get; set; }
        public double SalaryOfMonth { get; set; }
    }
}
