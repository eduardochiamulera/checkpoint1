using System;
using System.Collections.Generic;
using Cursos.Application.Common;
using Cursos.Application.Enrollments.CreateEnrollment;

namespace Cursos.Application.Enrollments.GetEnrollmentsByStudent;

public record GetEnrollmentsByStudentQuery(Guid StudentId) : IQuery<IEnumerable<EnrollmentDto>>;
