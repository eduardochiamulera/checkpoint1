using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record EnrollmentAdminRequest(
    [Required] int StudentId,
    [Required] int CourseId
);
