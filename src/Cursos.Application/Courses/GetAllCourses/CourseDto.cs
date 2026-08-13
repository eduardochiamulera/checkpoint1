namespace Cursos.Application.Courses.GetAllCourses;

public record CourseDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Instructor,
    int DurationHours,
    bool IsActive,
    DateTime CreatedAt
);
