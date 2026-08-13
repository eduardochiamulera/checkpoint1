using Cursos.Domain.Entities;
using Cursos.Domain.Payments;

namespace Cursos.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Check if already has data
        if (context.Courses.Any())
        {
            return;
        }
        
        // Seed Courses
        var courses = new List<Course>
        {
            new Course(
                name: "ASP.NET Core Web API",
                description: "Learn to build RESTful APIs with ASP.NET Core",
                price: 299.90m,
                instructor: "John Doe",
                durationHours: 20),
            
            new Course(
                name: "Entity Framework Core",
                description: "Master EF Core for data access in .NET",
                price: 249.90m,
                instructor: "Jane Smith",
                durationHours: 15),
            
            new Course(
                name: "Clean Architecture",
                description: "Build maintainable software with Clean Architecture",
                price: 349.90m,
                instructor: "Bob Johnson",
                durationHours: 25)
        };
        
        context.Courses.AddRange(courses);
        
        // Seed Students
        var students = new List<Student>
        {
            new Student(
                name: "Alice Silva",
                email: "alice@example.com",
                phone: "+55 11 99999-1111",
                birthDate: new DateTime(1990, 5, 15)),
            
            new Student(
                name: "Bruno Santos",
                email: "bruno@example.com",
                phone: "+55 11 99999-2222",
                birthDate: new DateTime(1992, 8, 20)),
            
            new Student(
                name: "Carla Oliveira",
                email: "carla@example.com",
                phone: "+55 11 99999-3333",
                birthDate: new DateTime(1995, 12, 10))
        };
        
        context.Students.AddRange(students);
        
        await context.SaveChangesAsync();
        
        // Seed Enrollments
        var enrollments = new List<Enrollment>
        {
            new Enrollment(students[0].Id, courses[0].Id),
            new Enrollment(students[1].Id, courses[0].Id),
            new Enrollment(students[2].Id, courses[1].Id)
        };
        
        context.Enrollments.AddRange(enrollments);
        
        await context.SaveChangesAsync();
    }
}
