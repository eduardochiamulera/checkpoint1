using System;
using Cursos.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cursos.Infrastructure.Data.Configurations;

public class PaymentGatewayTransactionConfiguration : IEntityTypeConfiguration<PaymentGatewayTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentGatewayTransaction> builder)
    {
        builder.ToTable("PaymentGatewayTransactions");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.PaymentId)
            .IsRequired();
        
        builder.Property(t => t.GatewayType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(t => t.TransactionId)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(t => t.TransactionId)
            .IsUnique();
        
        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("BRL");
        
        builder.Property(t => t.RawResponse)
            .HasMaxLength(4000);
        
        builder.Property(t => t.ErrorMessage)
            .HasMaxLength(1000);
        
        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(t => t.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// Entity for transaction history
public class PaymentGatewayTransaction
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public string GatewayType { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public string? RawResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
