namespace Cursos.Models;

public record StudentResponse(int id, string nome, string email, IEnumerable<EnrollmentsResponse> enrollments);
