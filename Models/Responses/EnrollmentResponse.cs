namespace Cursos.Models;

/// <summary>Dados de uma matrícula.</summary>
public record EnrollmentResponse(
    /// <example>10</example>
    int Id,
    /// <example>1</example>
    int StudentId,
    /// <example>João da Silva</example>
    string NomeEstudante,
    /// <example>2</example>
    int CourseId,
    /// <example>Introdução ao C#</example>
    string TituloCurso,
    /// <example>Ativo</example>
    string Status,
    /// <example>2026-06-25T10:00:00Z</example>
    DateTime DataMatricula,
    /// <example>null</example>
    DateTime? DataCancelamento
);