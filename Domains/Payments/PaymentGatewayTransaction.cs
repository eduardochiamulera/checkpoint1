using Cursos.Domain.Exceptions;

namespace Cursos.Domains.Payments;

public enum GatewayTransactionOperation
{
    Create = 1,
    Confirm = 2,
    Refund = 3
}

public sealed class PaymentGatewayTransaction
{
    private PaymentGatewayTransaction()
    {
        ExternalPaymentId = null!;
    }

    private PaymentGatewayTransaction(
        Guid paymentId,
        GatewayTransactionOperation operation,
        string externalPaymentId,
        string? receipt,
        GatewayOperationStatus status,
        DateTimeOffset occurredAt,
        string? errorCode,
        string? errorMessage)
    {
        PaymentId = paymentId;
        Operation = operation;
        ExternalPaymentId = externalPaymentId;
        Receipt = receipt;
        Status = status;
        OccurredAt = occurredAt;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public long Id { get; private set; }
    public Guid PaymentId { get; private set; }
    public GatewayTransactionOperation Operation { get; private set; }
    public string ExternalPaymentId { get; private set; }
    public string? Receipt { get; private set; }
    public GatewayOperationStatus Status { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static PaymentGatewayTransaction Create(
        Guid paymentId,
        GatewayTransactionOperation operation,
        string externalPaymentId,
        string? receipt,
        GatewayOperationStatus status,
        DateTimeOffset occurredAt,
        string? errorCode = null,
        string? errorMessage = null)
    {
        if (paymentId == Guid.Empty)
            throw new DomainException(
                "invalid_payment",
                "O pagamento é obrigatório.");

        if (string.IsNullOrWhiteSpace(externalPaymentId))
            throw new DomainException(
                "missing_external_payment_id",
                "O identificador externo do pagamento é obrigatório.");

        return new PaymentGatewayTransaction(
            paymentId,
            operation,
            externalPaymentId.Trim(),
            string.IsNullOrWhiteSpace(receipt) ? null : receipt.Trim(),
            status,
            occurredAt,
            errorCode,
            errorMessage);
    }
}