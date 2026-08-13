using Cursos.Application.Common;

namespace Cursos.Application.Enrollments.GetEnrollmentsByStudent;

public record GetEnrollmentsByStudentQuery(Guid StudentId) : IQuery<IEnumerable<EnrollmentDto>>;
