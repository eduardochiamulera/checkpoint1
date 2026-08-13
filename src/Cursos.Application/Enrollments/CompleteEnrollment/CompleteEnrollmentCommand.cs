using System;
using Cursos.Application.Common;
using Cursos.Application.Enrollments.CreateEnrollment;

namespace Cursos.Application.Enrollments.CompleteEnrollment;

public record CompleteEnrollmentCommand(Guid Id) : ICommand<EnrollmentDto>;
