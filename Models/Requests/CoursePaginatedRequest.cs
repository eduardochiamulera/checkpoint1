namespace Cursos.Models;

/// <summary>Parâmetros de paginação e filtro para listagem de cursos.</summary>
public record CoursePaginatedRequest(
    /// <summary>Número da página. Mínimo: 1.</summary>
    /// <example>1</example>
    int page = 1,
    /// <summary>Quantidade de itens por página. Máximo recomendado: 50.</summary>
    /// <example>10</example>
    int size = 10,
    /// <summary>Filtro por texto no título ou descrição.</summary>
    /// <example>C#</example>
    string? filter = null,
    /// <summary>Campo para ordenação. Ex: titulo, cargaHoraria.</summary>
    /// <example>titulo</example>
    string? orderBy = null,
    /// <summary>Direção da ordenação: ASC ou DESC.</summary>
    /// <example>ASC</example>
    string direction = "ASC")
{
    /// <summary>Filtro por categorias. Pode informar múltiplas.</summary>
    public IEnumerable<string> categories { get; init; } = Enumerable.Empty<string>();
}
