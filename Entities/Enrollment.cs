namespace Cursos.Entites;

public class Enrollment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime DataMatricula { get; set; }
    public DateTime? DataCancelamento { get; set; }
    public string UsuarioCriacao { get; set; } = string.Empty;
    public virtual Student Student { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
    public string UsuarioAlteracao { get; set; } = string.Empty;
}

public enum EnrollmentStatus { Ativo, Cancelado }