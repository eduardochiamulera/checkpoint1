using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

/// <summary>Dados para matrícula de estudante por um Admin.</summary>
public record EnrollmentAdminRequest(
    /// <example>1</example>
    [Required] int StudentId,
    /// <example>2</example>
    [Required] int CourseId
);
