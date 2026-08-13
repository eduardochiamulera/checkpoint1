using System;
using Cursos.Application.Common;

namespace Cursos.Application.Enrollments.CreateEnrollment;

public record CreateEnrollmentCommand(
    Guid StudentId,
    Guid CourseId
) : ICommand<EnrollmentDto>;
