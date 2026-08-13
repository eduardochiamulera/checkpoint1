using System.Collections.Generic;
using Cursos.Application.Common;

namespace Cursos.Application.Courses.GetAllCourses;

public record GetAllCoursesQuery(
    int Skip = 0,
    int Take = 50
) : IQuery<IEnumerable<CourseDto>>;
