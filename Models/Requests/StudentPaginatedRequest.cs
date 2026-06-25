namespace Cursos.Models;

public record StudentPaginatedRequest(
    int page = 1,
    int size = 10,
    string? filter = null,
    string? orderBy = null,
    string direction = "ASC");