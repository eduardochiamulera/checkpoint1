using Cursos.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", table =>
        {
            table.HasCheckConstraint(
                "CK_Payments_Amount_Positive",
                "[Amount] > 0");

            table.HasCheckConstraint(
                "CK_Payments_Currency_Iso4217",
                "LEN([Currency]) = 3 AND [Currency] = UPPER([Currency])");
        });

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.StudentId)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(payment => payment.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(payment => payment.CreatedAt)
            .HasColumnType("datetimeoffset(3)")
            .IsRequired();

        builder.Property(payment => payment.UpdatedAt)
            .HasColumnType("datetimeoffset(3)")
            .IsRequired();

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(payment => payment.StudentId)
            .HasPrincipalKey(student => student.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(payment => payment.UserId)
            .HasPrincipalKey(user => user.Id)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne<Enrollment>()
            .WithMany()
            .HasForeignKey(payment => payment.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Payments_Enrollments_EnrollmentId");

        builder.HasIndex(payment => payment.EnrollmentId)
            .HasDatabaseName("IX_Payments_EnrollmentId");

        builder.HasIndex(payment => payment.Status)
            .HasDatabaseName("IX_Payments_Status");

        builder.HasIndex(payment => payment.EnrollmentId)
            .IsUnique()
            .HasFilter("[Status] IN (1, 2)")
            .HasDatabaseName("UX_Payments_Active_EnrollmentId");

        builder.HasIndex(payment => payment.StudentId)
            .HasDatabaseName("IX_Payments_StudentId");

        builder.HasIndex(payment => payment.UserId)
            .HasDatabaseName("IX_Payments_UserId");

        builder.HasIndex(payment => payment.EnrollmentId)
            .HasDatabaseName("IX_Payments_EnrollmentId");

        builder.HasIndex(payment => payment.Status)
            .HasDatabaseName("IX_Payments_Status");

        builder.HasIndex(payment =>
                new
                {
                    payment.UserId,
                    payment.IdempotencyKey
                })
            .IsUnique()
            .HasDatabaseName("UX_Payments_UserId_IdempotencyKey");

        builder.HasIndex(payment => payment.EnrollmentId)
            .IsUnique()
            .HasFilter("[Status] IN (1, 2)")
            .HasDatabaseName("UX_Payments_Active_EnrollmentId");
    }
}