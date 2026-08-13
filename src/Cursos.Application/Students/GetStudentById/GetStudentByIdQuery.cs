using Cursos.Application.Common;

namespace Cursos.Application.Students.GetStudentById;

public record GetStudentByIdQuery(Guid Id) : IQuery<StudentDto?>;
