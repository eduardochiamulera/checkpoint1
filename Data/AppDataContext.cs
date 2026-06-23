
using Cursos.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
        .HasIndex(s => s.Email)
        .IsUnique();

        // Enrollment - matrícula única por aluno/curso
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();

        // Enrollment - status default
        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion<string>()
            .HasDefaultValue(EnrollmentStatus.Ativo);

        base.OnModelCreating(modelBuilder);
    }
}