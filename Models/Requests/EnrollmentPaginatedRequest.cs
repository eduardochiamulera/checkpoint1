namespace Cursos.Models;

/// <summary>Parâmetros de paginação e filtro para listagem de matrículas.</summary>
public record EnrollmentPaginatedRequest(
    /// <summary>Número da página. Mínimo: 1.</summary>
    /// <example>1</example>
    int Page = 1,
    /// <summary>Quantidade de itens por página.</summary>
    /// <example>10</example>
    int Size = 10,
    /// <summary>Filtro por status da matrícula: Ativo ou Cancelado.</summary>
    /// <example>Ativo</example>
    string? Status = null
);