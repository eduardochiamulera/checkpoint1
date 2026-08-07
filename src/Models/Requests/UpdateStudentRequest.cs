using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

/// <summary>Dados para atualização do perfil de um estudante.</summary>
public record UpdateStudentRequest(
    /// <example>joao.novo@email.com</example>
    [Required][EmailAddress] string Email,
    /// <example>João da Silva Atualizado</example>
    [Required] string NomeCompleto
);
