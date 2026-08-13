using Cursos.Application.Common;

namespace Cursos.Application.Enrollments.GetEnrollmentsByCourse;

public record GetEnrollmentsByCourseQuery(Guid CourseId) : IQuery<IEnumerable<EnrollmentDto>>;
