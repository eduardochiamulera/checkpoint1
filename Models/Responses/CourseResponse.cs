namespace Cursos.Models;

/// <summary>Dados resumidos de um curso.</summary>
public record CourseResponse(
    /// <example>1</example>
    int id,
    /// <example>Introdução ao C#</example>
    string titulo,
    /// <example>Curso voltado para iniciantes em C# e .NET</example>
    string descricao,
    /// <example>Programação</example>
    string categoria,
    /// <example>120</example>
    int cargaHoraria
);