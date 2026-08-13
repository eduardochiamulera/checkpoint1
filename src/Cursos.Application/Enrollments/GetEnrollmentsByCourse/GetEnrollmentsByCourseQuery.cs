using System;
using System.Collections.Generic;
using Cursos.Application.Common;
using Cursos.Application.Enrollments.CreateEnrollment;

namespace Cursos.Application.Enrollments.GetEnrollmentsByCourse;

public record GetEnrollmentsByCourseQuery(Guid CourseId) : IQuery<IEnumerable<EnrollmentDto>>;
