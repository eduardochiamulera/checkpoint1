namespace Cursos.Models;

public record CoursePaginatedRequest(
    int page = 1,
    int size = 10,
    string? filter = null,
    string? orderBy = null,
    string direction = "ASC")
{
    public IEnumerable<string> categories { get; init; } = Enumerable.Empty<string>();
}
