namespace PayrollCalculator.Models
{
    public class EmployeePayroll
    {
        public string Employee { get; set; }
        public double RegularHours { get; set; }
        public double OvertimeHours { get; set; }
        public double DoubletimeHours { get; set; }
        public double WageTotal { get; set; }
        public double BenefitTotal { get; set; }
    }
}
