using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

/// <summary>Credenciais para autenticação.</summary>
public record LoginRequest(
    /// <example>admin@cursos.com</example>
    [Required][EmailAddress] string Email,
    /// <example>Senha@123</example>
    [Required] string Password
);