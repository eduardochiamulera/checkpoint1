using Cursos.Domains.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cursos.Data.Configurations;

public sealed class PaymentGatewayTransactionConfiguration
    : IEntityTypeConfiguration<PaymentGatewayTransaction>
{
    public void Configure(
        EntityTypeBuilder<PaymentGatewayTransaction> builder)
    {
        builder.ToTable("PaymentGatewayTransactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.ExternalPaymentId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(transaction => transaction.Receipt)
            .HasMaxLength(200);

        builder.Property(transaction => transaction.ErrorCode)
            .HasMaxLength(100);

        builder.Property(transaction => transaction.ErrorMessage)
            .HasMaxLength(500);

        builder.Property(transaction => transaction.Operation)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(transaction => transaction.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(transaction => transaction.OccurredAt)
            .HasConversion(new UtcDateTimeOffsetConverter())
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(transaction => transaction.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(transaction => transaction.PaymentId)
            .HasDatabaseName(
                "IX_PaymentGatewayTransactions_PaymentId");

        builder.HasIndex(transaction =>
                new
                {
                    transaction.ExternalPaymentId,
                    transaction.Operation
                })
            .IsUnique()
            .HasDatabaseName(
                "UX_PaymentGatewayTransactions_External_Operation");
    }
}