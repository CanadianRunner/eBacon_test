using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PayrollCalculator.Models;

namespace PayrollCalculator.Data
{
    public static class DataInitializer
    {
        public static List<Job> LoadJobMeta(string jobMetaFilePath)
        {
            if (!File.Exists(jobMetaFilePath))
            {
                throw new FileNotFoundException($"The file '{jobMetaFilePath}' does not exist.");
            }

            string jsonData;
            try
            {
                jsonData = File.ReadAllText(jobMetaFilePath);
            }
            catch (IOException ex)
            {
                throw new IOException($"An error occurred while reading the file '{jobMetaFilePath}': {ex.Message}", ex);
            }

            List<RawJob> rawJobs;
            try
            {
                rawJobs = JsonConvert.DeserializeObject<List<RawJob>>(jsonData);
            }
            catch (JsonException ex)
            {
                throw new JsonException("Failed to deserialize JSON data for job metadata. Ensure the JSON structure is correct.", ex);
            }

            var jobMeta = new List<Job>();
            foreach (var job in rawJobs)
            {
                jobMeta.Add(new Job
                {
                    JobName = job.Job,
                    Rate = job.Rate,
                    BenefitsRate = job.BenefitsRate
                });
            }

            return jobMeta;
        }

        public static List<EmployeeData> LoadEmployeeData(string employeeDataFilePath)
        {
            if (!File.Exists(employeeDataFilePath))
            {
                throw new FileNotFoundException($"The file '{employeeDataFilePath}' does not exist.");
            }

            string jsonData;
            try
            {
                jsonData = File.ReadAllText(employeeDataFilePath);
            }
            catch (IOException ex)
            {
                throw new IOException($"An error occurred while reading the file '{employeeDataFilePath}': {ex.Message}", ex);
            }

            List<RawEmployee> rawEmployees;
            try
            {
                rawEmployees = JsonConvert.DeserializeObject<List<RawEmployee>>(jsonData);
            }
            catch (JsonException ex)
            {
                throw new JsonException("Failed to deserialize JSON data for employee data. Ensure the JSON structure is correct.", ex);
            }

            var employeeData = new List<EmployeeData>();
            foreach (var employee in rawEmployees)
            {
                var timePunches = new List<TimePunch>();
                foreach (var punch in employee.TimePunch)
                {
                    if (DateTime.TryParse(punch.Start, out DateTime start) && DateTime.TryParse(punch.End, out DateTime end))
                    {
                        timePunches.Add(new TimePunch
                        {
                            Job = punch.Job,
                            Start = start,
                            End = end
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Invalid time format for employee '{employee.Employee}'. Start: '{punch.Start}', End: '{punch.End}'. Skipping this punch.");
                    }
                }

                employeeData.Add(new EmployeeData
                {
                    Employee = employee.Employee,
                    TimePunches = timePunches
                });
            }

            return employeeData;
        }
    }

    public class RawJob
    {
        public string Job { get; set; }
        public double Rate { get; set; }
        public double BenefitsRate { get; set; }
    }

    public class RawEmployee
    {
        public string Employee { get; set; }
        public List<RawTimePunch> TimePunch { get; set; }
    }

    public class RawTimePunch
    {
        public string Job { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
