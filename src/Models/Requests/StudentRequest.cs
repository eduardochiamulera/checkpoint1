using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record StudentRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string nomeCompleto,
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress]
    string email,
    [Required(ErrorMessage = "A senha é obrigatória.")]
    string password

);
