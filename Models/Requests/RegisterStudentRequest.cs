using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record RegisterStudentRequest(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] string Password,
    [Required] string NomeCompleto
);

public record UpdateStudentRequest(
    [Required] [EmailAddress] string Email,
    [Required] string NomeCompleto
);
