using Cursos.Domains;

namespace Cursos.Models;

public record EnrollmentsResponse(int id, int courseId, int studentId, EnrollmentStatus status, DateTime dataMatricula);