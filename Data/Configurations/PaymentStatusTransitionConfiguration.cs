using Cursos.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cursos.Data.Configurations;

public sealed class PaymentStatusTransitionConfiguration
    : IEntityTypeConfiguration<PaymentStatusTransition>
{
    public void Configure(EntityTypeBuilder<PaymentStatusTransition> builder)
    {
        builder.ToTable("PaymentStatusTransitions", table =>
        {
            table.HasCheckConstraint(
                "CK_PaymentStatusTransitions_DifferentStatuses",
                "[From] <> [To]");
        });

        builder.Property<long>("Id")
            .ValueGeneratedOnAdd();

        builder.HasKey("Id");

        builder.Property(transition => transition.From)
            .HasColumnName("FromStatus")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(transition => transition.To)
            .HasColumnName("ToStatus")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(transition => transition.OccurredAt)
            .HasColumnType("datetimeoffset(3)")
            .IsRequired();

        builder.Property(transition => transition.Reason)
            .HasMaxLength(500);

        builder.HasIndex("PaymentId", nameof(PaymentStatusTransition.OccurredAt))
            .HasDatabaseName("IX_PaymentStatusTransitions_Payment_OccurredAt");
    }
}