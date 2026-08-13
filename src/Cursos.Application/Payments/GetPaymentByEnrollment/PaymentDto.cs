using System;

namespace Cursos.Application.Payments.GetPaymentByEnrollment;

public record PaymentDto(
    Guid Id,
    Guid EnrollmentId,
    decimal Amount,
    string Currency,
    string Status,
    string? TransactionId,
    DateTime CreatedAt
);
