using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;


/// <summary>Dados para auto-matrícula do estudante autenticado.</summary>
public record EnrollmentStudentRequest(
    /// <example>2</example>
    [Required] int CourseId
);