namespace Cursos.Application.Enrollments.CreateEnrollment;

public record EnrollmentDto(
    Guid Id,
    Guid StudentId,
    Guid CourseId,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletionDate,
    DateTime CreatedAt
);
