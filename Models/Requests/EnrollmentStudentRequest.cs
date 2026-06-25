using System.ComponentModel.DataAnnotations;

namespace Cursos.Models;

public record EnrollmentStudentRequest(
    [Required] int CourseId
);
