using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

/// <summary>Dados para registro de estudante.</summary>
public record RegisterStudentRequest(
    /// <example>joao.silva@email.com</example>
    [Required][EmailAddress] string Email,
    /// <example>Senha@123</example>
    [Required][MinLength(8)] string Password,
    /// <example>João da Silva</example>
    [Required] string NomeCompleto
);
