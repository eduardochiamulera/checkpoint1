namespace Cursos.Models;

/// <summary>Resposta paginada genérica.</summary>
public record PaginatedResponse<T>(
    /// <summary>Lista de itens da página atual.</summary>
    IEnumerable<T> items,
    /// <summary>Página atual.</summary>
    /// <example>1</example>
    int page,
    /// <summary>Quantidade de itens por página.</summary>
    /// <example>10</example>
    int pageSize,
    /// <summary>Total de registros encontrados.</summary>
    /// <example>42</example>
    int total)
{
    /// <summary>Total de páginas.</summary>
    public int totalPages => (int)Math.Ceiling((double)total / pageSize);

    /// <summary>Indica se existe página anterior.</summary>
    public bool HasPrevious => page > 1;

    /// <summary>Indica se existe próxima página.</summary>
    public bool HasNext => page < totalPages;
}