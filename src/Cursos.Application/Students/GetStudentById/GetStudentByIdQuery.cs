using System;
using Cursos.Application.Common;
using Cursos.Application.Students.CreateStudent;

namespace Cursos.Application.Students.GetStudentById;

public record GetStudentByIdQuery(Guid Id) : IQuery<StudentDto?>;
