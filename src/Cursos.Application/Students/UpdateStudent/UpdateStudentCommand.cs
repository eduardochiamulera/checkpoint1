using Cursos.Application.Common;

namespace Cursos.Application.Students.UpdateStudent;

public record UpdateStudentCommand(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate
) : ICommand<StudentDto>;
