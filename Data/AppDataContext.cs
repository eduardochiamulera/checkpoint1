
using Cursos.Data.Configurations;
using Cursos.Domain.Payments;
using Cursos.Domains;
using Cursos.Domains.Payments;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentStatusTransition> PaymentStatusTransitions => Set<PaymentStatusTransition>();

    public DbSet<PaymentGatewayTransaction> PaymentGatewayTransactions => Set<PaymentGatewayTransaction>();

    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }

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

        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentStatusTransitionConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentGatewayTransactionConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}