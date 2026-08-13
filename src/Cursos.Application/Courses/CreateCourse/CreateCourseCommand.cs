using Cursos.Application.Common;

namespace Cursos.Application.Courses.CreateCourse;

public record CreateCourseCommand(
    string Name,
    string Description,
    decimal Price,
    string Instructor,
    int DurationHours
) : ICommand<CourseDto>;
