using System;

namespace Cursos.Application.Payments.ProcessPayment;

public record PaymentResultDto(
    bool Success,
    Guid? PaymentId,
    string? TransactionId,
    string? ErrorMessage
);
