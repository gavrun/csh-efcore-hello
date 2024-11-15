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
            //add data explicitly
            // context.Students.Add(new Student { Name = "John Doe", EnrollmentDate = DateTime.Now });
            // context.Students.Add(new Student { Name = "Jane Smith", EnrollmentDate = DateTime.Now.AddDays(-5) });
            // context.SaveChanges();

            //clear table, reset index, reset identity
            context.Students.RemoveRange(context.Students);
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.[Students]', RESEED, 0)");

            //add data randomizing
            var firstNames = new[] { "John", "Jane", "Alice", "Bob", "Charlie", "Diana", "Edward", "Fiona", "George", "Hannah", "Ian", "Julia", "Kevin", "Laura", "Michael", "Nina", "Oliver", "Paula", "Quincy", "Rachel" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };

            var random = new Random();

            for (int i = 0; i < 20; i++)
                {
                    //name name unique and append data within laslt 30 days range 
                    var firstName = firstNames[random.Next(firstNames.Length)];
                    var lastName = lastNames[random.Next(lastNames.Length)];
                    var fullName = $"{firstName} {lastName}"; 
                    var enrollmentDate = DateTime.Now.AddDays(-random.Next(0, 30));

                    context.Students.Add(new Student { Name = fullName, EnrollmentDate = enrollmentDate });
                }
            context.SaveChanges();

            //read data
            var students = context.Students.ToList();
            Console.WriteLine("Students:");

            foreach (var student in students)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Enrollment Date: {student.EnrollmentDate}");
            }

            //run SQL query
            Console.WriteLine("Executing SQL Query:\nSELECT * FROM Students WHERE Name LIKE '%Ja%'");
            var sqlStudents = context.Students
                .FromSqlRaw("SELECT * FROM dbo.[Students] WHERE Name LIKE '%Ja%'")
                .ToList();

            Console.WriteLine("SQL Query Results:");
            foreach (var student in sqlStudents)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
            }
        }
    }
}
