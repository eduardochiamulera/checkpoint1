using System;

namespace Cursos.Application.Students.CreateStudent;

public record StudentDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate,
    bool IsActive,
    DateTime CreatedAt
);
