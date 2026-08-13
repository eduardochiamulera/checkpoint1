using System;
using Cursos.Application.Common;
using Cursos.Application.Enrollments.CreateEnrollment;

namespace Cursos.Application.Enrollments.CancelEnrollment;

public record CancelEnrollmentCommand(Guid Id) : ICommand<EnrollmentDto>;
