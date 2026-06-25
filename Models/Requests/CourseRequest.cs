using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

// CourseRequest.cs
/// <summary>Dados para criação ou atualização de curso.</summary>
public record CourseRequest(
    /// <example>Introdução ao C#</example>
    [Required][StringLength(100, MinimumLength = 3)] string titulo,
    /// <example>Curso voltado para iniciantes em C# e .NET</example>
    [Required][StringLength(500, MinimumLength = 10)] string descricao,
    /// <example>Programação</example>
    [Required][StringLength(50, MinimumLength = 3)] string categoria,
    /// <example>120</example>
    [Range(30, int.MaxValue)] int cargaHoraria
);
