using Cursos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cursos.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(s => s.Email)
            .IsUnique();
        
        builder.Property(s => s.Phone)
            .HasMaxLength(20);
        
        builder.Property(s => s.BirthDate)
            .IsRequired();
        
        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
    }
}
