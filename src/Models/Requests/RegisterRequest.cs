using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

// RegisterRequest.cs
/// <summary>Dados para registro de usuário Admin ou Instructor.</summary>
public record RegisterRequest(
    /// <example>admin@cursos.com</example>
    [Required][EmailAddress] string Email,
    /// <example>Senha@123</example>
    [Required][MinLength(8)] string Password,
    /// <example>Admin</example>
    [Required] string Role
);
