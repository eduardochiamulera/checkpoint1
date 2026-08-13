using Cursos.Application.Common;

namespace Cursos.Application.Enrollments.CompleteEnrollment;

public record CompleteEnrollmentCommand(Guid Id) : ICommand<EnrollmentDto>;
