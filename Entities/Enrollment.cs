namespace Cursos.Entites;

public class Enrollment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public bool Status { get; set; }
    public string Email { get; set; }
    public DateTime DataMatricula { get; set; }
    public virtual Student Student {get; set;}
    public virtual Course Course {get; set;}
}