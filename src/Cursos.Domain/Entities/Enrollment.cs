using Cursos.Domain.Common;

namespace Cursos.Domain.Entities;

public class Enrollment : Entity
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    
    public Student Student { get; private set; } = null!;
    public Course Course { get; private set; } = null!;
    
    public Enrollment(Guid studentId, Guid courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
        Status = EnrollmentStatus.Active;
        EnrollmentDate = DateTime.UtcNow;
    }
    
    public void Complete()
    {
        Status = EnrollmentStatus.Completed;
        CompletionDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Cancel()
    {
        Status = EnrollmentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum EnrollmentStatus
{
    Active,
    Completed,
    Cancelled
}
