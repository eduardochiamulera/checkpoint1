using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record CourseRequest(
    [Required(ErrorMessage = "O título é obrigatório.")]
    string titulo, 
    [Required(ErrorMessage = "A descricao é obrigatória.")]
    string descricao, 
    [Required(ErrorMessage = "A categoria é obrigatória.")]
    string categoria, 
    [Range(30, int.MaxValue, ErrorMessage = "A carga horária deve ser entre 30 e 999999 minutos.")]
    int cargaHoraria);

public record StudentRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string nomeCompleto,
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress]
    string email,
    [Required(ErrorMessage = "A senha é obrigatória.")]
    string password

);
