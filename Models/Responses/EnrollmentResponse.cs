namespace Cursos.Models;

public record EnrollmentResponse(
    int Id,
    int StudentId,
    string NomeEstudante,
    int CourseId,
    string TituloCurso,
    string Status,
    DateTime DataMatricula,
    DateTime? DataCancelamento
);