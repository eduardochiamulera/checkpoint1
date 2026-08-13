using System;
using Cursos.Application.Common;
using Cursos.Application.Students.CreateStudent;

namespace Cursos.Application.Students.UpdateStudent;

public record UpdateStudentCommand(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate
) : ICommand<StudentDto>;
