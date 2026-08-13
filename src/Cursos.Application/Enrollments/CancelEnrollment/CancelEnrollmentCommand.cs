using Cursos.Application.Common;

namespace Cursos.Application.Enrollments.CancelEnrollment;

public record CancelEnrollmentCommand(Guid Id) : ICommand<EnrollmentDto>;
