namespace Cursos.Models;

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
