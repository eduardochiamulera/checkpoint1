namespace Cursos.Models;

/// <summary>Dados de perfil de um estudante.</summary>
public record StudentResponse(
    /// <example>1</example>
    int id,
    /// <example>João da Silva</example>
    string nome,
    /// <example>joao.silva@email.com</example>
    string email,
    /// <summary>Lista de matrículas do estudante.</summary>
    IEnumerable<EnrollmentsResponse> enrollments
);