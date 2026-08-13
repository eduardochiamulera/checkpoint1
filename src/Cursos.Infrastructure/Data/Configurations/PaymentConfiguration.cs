using System;
using Cursos.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cursos.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        
        builder.HasKey(p => p.Id);
        
        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            
            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("BRL");
        });
        
        builder.Property(p => p.EnrollmentId)
            .IsRequired();
        
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v));
        
        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(100);
        
        builder.Property(p => p.PaymentMethodType)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), v));
        
        builder.HasIndex(p => p.EnrollmentId);
        builder.HasIndex(p => p.GatewayTransactionId);
    }
}
