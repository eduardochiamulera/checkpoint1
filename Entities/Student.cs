using Microsoft.AspNetCore.Identity;

namespace Cursos.Entites;

public class Student
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public DateTime DataCadastro { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; }
    public bool IsDeleted { get; set; }
    public string UserId { get; set; }
    public virtual IdentityUser User { get; set; }
    public string UsuarioCriacao { get; set; }

    public DateTime? DataAlteracao { get; set; }

    public string? UsuarioAlteracao { get; set; }
}
