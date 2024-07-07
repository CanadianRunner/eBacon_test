using System.Collections.Generic;

namespace PayrollCalculator.Models
{
    public class EmployeeData
    {
        public string Employee { get; set; }
        public List<TimePunch> TimePunches { get; set; }
    }
}
