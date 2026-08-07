using Cursos.Domain.Payments;

namespace Cursos.Domains.Payments;

public sealed record GatewayCreateRequest(
    Guid PaymentId,
    decimal Amount,
    string Currency,
    PaymentMethodType Method,
    string IdempotencyKey);

public sealed record GatewayConfirmRequest(
    Guid PaymentId,
    string ExternalPaymentId,
    string IdempotencyKey);

public sealed record GatewayRefundRequest(
    Guid PaymentId,
    string ExternalPaymentId,
    string IdempotencyKey,
    string Reason);

public enum GatewayOperationStatus
{
    Succeeded = 1,
    Failed = 2,
    Declined = 3,
    Timeout = 4
}

public sealed record GatewayCreateResult(
    GatewayOperationStatus Status,
    string? ExternalPaymentId,
    string? Receipt,
    DateTimeOffset OccurredAt,
    string? ErrorCode,
    string? ErrorMessage)
{
    public bool Succeeded =>
        Status == GatewayOperationStatus.Succeeded;
}

public sealed record GatewayOperationResult(
    GatewayOperationStatus Status,
    string? ExternalPaymentId,
    string? Receipt,
    DateTimeOffset OccurredAt,
    string? ErrorCode,
    string? ErrorMessage)
{
    public bool Succeeded =>
        Status == GatewayOperationStatus.Succeeded;
}