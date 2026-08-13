using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cursos.Infrastructure.Data.Configurations;

public class PaymentStatusTransitionConfiguration : IEntityTypeConfiguration<PaymentStatusTransition>
{
    public void Configure(EntityTypeBuilder<PaymentStatusTransition> builder)
    {
        builder.ToTable("PaymentStatusTransitions");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.PaymentId)
            .IsRequired();
        
        builder.Property(t => t.FromStatus)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(t => t.ToStatus)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(t => t.TransitionedAt)
            .IsRequired();
        
        builder.Property(t => t.Reason)
            .HasMaxLength(500);
        
        builder.Property(t => t.UserId)
            .IsRequired(false);
        
        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(t => t.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(t => t.PaymentId);
    }
}

// Entity for status change history
public class PaymentStatusTransition
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public DateTime TransitionedAt { get; set; }
    public string? Reason { get; set; }
    public Guid? UserId { get; set; }
}
