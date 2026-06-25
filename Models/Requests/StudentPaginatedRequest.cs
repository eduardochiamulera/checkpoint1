namespace Cursos.Models;

/// <summary>Parâmetros de paginação e filtro para listagem de estudantes.</summary>
public record StudentPaginatedRequest(
    /// <summary>Número da página. Mínimo: 1.</summary>
    /// <example>1</example>
    int page = 1,
    /// <summary>Quantidade de itens por página.</summary>
    /// <example>10</example>
    int size = 10,
    /// <summary>Filtro por nome ou email.</summary>
    /// <example>João</example>
    string? filter = null,
    /// <summary>Campo para ordenação. Ex: nome, email.</summary>
    /// <example>nome</example>
    string? orderBy = null,
    /// <summary>Direção da ordenação: ASC ou DESC.</summary>
    /// <example>ASC</example>
    string direction = "ASC");