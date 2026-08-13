using Cursos.Application.Common;

namespace Cursos.Application.Students.GetAllStudents;

public record GetAllStudentsQuery(
    int Skip = 0,
    int Take = 50
) : IQuery<IEnumerable<StudentDto>>;
