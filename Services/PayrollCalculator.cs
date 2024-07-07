using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PayrollCalculator.Models;

namespace PayrollCalculator.Services
{
    public class PayrollCalculator
    {
        private List<Job> Jobs;
        private List<EmployeeData> Employees;

        public PayrollCalculator(List<Job> jobs, List<EmployeeData> employees)
        {
            if (jobs == null || employees == null)
            {
                throw new ArgumentNullException("Jobs or Employees data cannot be null");
            }

            Jobs = jobs;
            Employees = employees;
        }

        public void CalculatePayroll()
        {
            var results = new Dictionary<string, object>();

            foreach (var employee in Employees)
            {
                double totalHours = 0;
                double regularHours = 0;
                double overtimeHours = 0;
                double doubletimeHours = 0;
                double totalWages = 0;
                double totalBenefits = 0;

                foreach (var punch in employee.TimePunches)
                {
                    var job = Jobs.Find(j => j.JobName == punch.Job);
                    if (job == null) 
                    {
                        Console.WriteLine($"Job '{punch.Job}' not found for employee '{employee.Employee}'. Skipping this punch.");
                        continue;
                    }

                    double hoursWorked = (punch.End - punch.Start).TotalHours;
                    if (hoursWorked < 0)
                    {
                        Console.WriteLine($"Invalid time punch for employee '{employee.Employee}'. Start time '{punch.Start}' is after end time '{punch.End}'. Skipping this punch.");
                        continue;
                    }

                    totalHours += hoursWorked;

                    double regularHoursForPunch = Math.Min(hoursWorked, Math.Max(0, 40 - regularHours));
                    regularHours += regularHoursForPunch;
                    hoursWorked -= regularHoursForPunch;

                    double overtimeHoursForPunch = Math.Min(hoursWorked, Math.Max(0, 48 - (regularHours + overtimeHours)));
                    overtimeHours += overtimeHoursForPunch;
                    hoursWorked -= overtimeHoursForPunch;

                    doubletimeHours += hoursWorked;

                    totalWages += regularHoursForPunch * job.Rate;
                    totalWages += overtimeHoursForPunch * job.Rate * 1.5;
                    totalWages += hoursWorked * job.Rate * 2;

                    totalBenefits += (regularHoursForPunch + overtimeHoursForPunch + hoursWorked) * job.BenefitsRate;
                }

                results[employee.Employee] = new
                {
                    employee = employee.Employee,
                    regular = regularHours.ToString("F4"),
                    overtime = overtimeHours.ToString("F4"),
                    doubletime = doubletimeHours.ToString("F4"),
                    wageTotal = totalWages.ToString("F4"),
                    benefitTotal = totalBenefits.ToString("F4")
                };
            }

            string jsonOutput = JsonConvert.SerializeObject(results, Formatting.Indented);
            Console.WriteLine(jsonOutput);
        }
    }
}
