using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password
);