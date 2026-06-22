namespace Cursos.Entites;

public class Student
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public DateTime DataCadastro { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; }
    public bool IsDeleted { get; set; }
}
