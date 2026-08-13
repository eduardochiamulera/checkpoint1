using Cursos.Application.Common;

namespace Cursos.Application.Students.CreateStudent;

public record CreateStudentCommand(
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate
) : ICommand<StudentDto>;
