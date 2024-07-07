using System;

namespace PayrollCalculator.Models
{
    public class TimePunch
    {
        public string Job { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
