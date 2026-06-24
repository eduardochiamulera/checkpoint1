using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record CourseRequest(
    [Required(ErrorMessage = "O título é obrigatório.")]
    string titulo, 
    [Required(ErrorMessage = "A descricao é obrigatória.")]
    string descricao, 
    [Required(ErrorMessage = "A categoria é obrigatória.")]
    string categoria, 
    [Range(30, int.MaxValue, ErrorMessage = "A carga horária deve ser entre 30 e 999999 minutos.")]
    int cargaHoraria);

public record CoursePaginatedRequest(
    int page = 1,
    int size = 10,
    string filter = null,
    string orderBy = null,
    string direction = "ASC")
{
    public IEnumerable<string> categories { get; init; } = Enumerable.Empty<string>();
}

public record CourseResponse(int id, string titulo, string descricao, string categoria, int cargaHoraria);

public record PaginatedResponse<T>(IEnumerable<T> items, int page, int pageSize, int total)
{
    public int totalPages
    {
        get
        {
            return (int)Math.Ceiling((double)total / pageSize);
        }
    }

    public bool HasPrevious
    {
        get
        {
            return page > 1;
        }
    }

    public bool HasNext
    {
        get
        {
            return page < totalPages;
        }
    }
};
