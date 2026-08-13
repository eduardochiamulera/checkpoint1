using Cursos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cursos.Infrastructure.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.StudentId)
            .IsRequired();
        
        builder.Property(e => e.CourseId)
            .IsRequired();
        
        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (EnrollmentStatus)Enum.Parse(typeof(EnrollmentStatus), v));
        
        builder.Property(e => e.EnrollmentDate)
            .IsRequired();
        
        builder.Property(e => e.CompletionDate)
            .IsRequired(false);
        
        // Relationships
        builder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Course)
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.CourseId);
        builder.HasIndex(e => new { e.StudentId, e.CourseId });
    }
}
