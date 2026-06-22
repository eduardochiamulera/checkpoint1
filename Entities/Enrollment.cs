namespace Cursos.Entites;

public class Enrollment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime DataMatricula { get; set; }
    public virtual Student Student {get; set;}
    public virtual Course Course {get; set;}
}

public enum EnrollmentStatus { Ativo, Cancelado }