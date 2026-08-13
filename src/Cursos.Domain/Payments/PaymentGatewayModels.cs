namespace Cursos.Domain.Payments;

public record PaymentResult(
    bool Success,
    string? TransactionId,
    string? ErrorMessage
);

public record RefundResult(
    bool Success,
    string? ErrorMessage
);
