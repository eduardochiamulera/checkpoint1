using System.Collections.Generic;
using Cursos.Application.Common;
using Cursos.Application.Students.CreateStudent;

namespace Cursos.Application.Students.GetAllStudents;

public record GetAllStudentsQuery(
    int Skip = 0,
    int Take = 50
) : IQuery<IEnumerable<StudentDto>>;
