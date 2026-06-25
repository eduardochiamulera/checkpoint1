namespace Cursos.Models;

public record EnrollmentPaginatedRequest(
    int Page = 1,
    int Size = 10,
    string? Status = null  // "Ativo" ou "Cancelado"
);