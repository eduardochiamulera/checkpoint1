using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record RegisterRequest(
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
    string Password,

    [Required(ErrorMessage = "O papel é obrigatório.")]
    string Role  // "Admin", "Instructor"
);

public record RegisterStudentRequest(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] string Password,
    [Required] string NomeCompleto
    // outros campos do Student...
);
