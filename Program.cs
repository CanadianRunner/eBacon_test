using System;
using PayrollCalculator.Data;
using PayrollCalculator.Services;

namespace PayrollCalculator
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                var jobMeta = DataInitializer.LoadJobMeta("Data/jobMeta.json");
                var employeeData = DataInitializer.LoadEmployeeData("Data/employeeData.json");

                var calculator = new PayrollCalculator.Services.PayrollCalculator(jobMeta, employeeData);

                calculator.CalculatePayroll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
