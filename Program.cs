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
            Console.WriteLine("Students list enrolled withing the last 30 days:");

            foreach (var student in students)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Enrollment Date: {student.EnrollmentDate}");
            }
            Console.WriteLine();

            //run SQL query, call chain
            Console.WriteLine("Executing SQL Query: find students with %Ja% in names");
            var sqlStudents = context.Students
                .FromSqlRaw("SELECT * FROM dbo.[Students] WHERE Name LIKE '%Ja%'")
                .ToList();

            Console.WriteLine("SQL Query Results:");
            foreach (var student in sqlStudents)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
            }

            // more stuff
            Console.WriteLine("Executing SQL Query: find students enrolled in the last 3 days");
            var recentStudents = context.Students
                .FromSqlRaw("SELECT * FROM dbo.[Students] WHERE EnrollmentDate >= DATEADD(DAY, -3, GETDATE())")
                .ToList();
            Console.WriteLine("SQL Query Results:");
            foreach (var student in recentStudents)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Enrollment Date: {student.EnrollmentDate}");
            }

            Console.WriteLine("Executing SQL Query: list newer students first");
            var sortedStudents = context.Students
                .FromSqlRaw("SELECT * FROM dbo.[Students] ORDER BY EnrollmentDate DESC")
                .ToList();
            Console.WriteLine("SQL Query Results:");
            foreach (var student in sortedStudents)
            {
                Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Enrollment Date: {student.EnrollmentDate}");
            }

            //LINQ vs SQL
            Console.WriteLine("Executing SQL Query: count students");
            var countStudents = context.Students.Count();
            //var countStudentsSql = context.Database.ExecuteSqlRaw("SELECT COUNT(*) FROM dbo.[Students]"); //bug
            //fixing bug
            int countStudentsSql = context.Database.ExecuteSqlRaw("SELECT COUNT(*) FROM dbo.[Students]");
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM dbo.[Students]";
                context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    result.Read();
                    countStudentsSql = result.GetInt32(0); //COUNT(*) to int
                }
            }
            Console.WriteLine($"Total number of students: {countStudents} or {countStudentsSql}");

            // utility class to database set
            Console.WriteLine("Executing SQL Query: group and count by first letter");
            var groupedStudents = context.StudentGroups
                .FromSqlRaw("SELECT LEFT(Name, 1) AS Initial, COUNT(*) AS Count FROM dbo.[Students] GROUP BY LEFT(Name, 1)")
                .ToList();
            Console.WriteLine("SQL Query Results:");
            foreach (var studentGroup in groupedStudents)
            {
                Console.WriteLine($"Initial: {studentGroup.Initial}, Count: {studentGroup.Count}");
            }

        }
    }
}
