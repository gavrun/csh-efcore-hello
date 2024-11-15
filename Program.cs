namespace csh_efcore_hello;

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, LINQ!");

        using (var context = new SchoolContext()) 
        {
            //add data
            context.Students.Add(new Student { Name = "John Doe", EnrollmentDate = DateTime.Now });
            context.Students.Add(new Student { Name = "Jane Smith", EnrollmentDate = DateTime.Now.AddDays(-5) });
            context.SaveChanges();

            //read data
            var students = context.Students.ToList();
            Console.WriteLine("Students:");

            foreach (var student in students)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Enrollment Date: {student.EnrollmentDate}");
            }

            //run SQL query
            Console.WriteLine("Executing SQL Query:\nSELECT * FROM Students WHERE Name LIKE '%Jane%'");
            var sqlStudents = context.Students
                .FromSqlRaw("SELECT * FROM Students WHERE Name LIKE '%Jane%'")
                .ToList();

            Console.WriteLine("SQL Query Results:");
            foreach (var student in sqlStudents)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
            }
        }
    }
}
