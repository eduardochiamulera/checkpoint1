using Cursos.Domain.Payments;

namespace Cursos.Models.Payments;

public sealed record PaymentResponse(
    Guid Id,
    int EnrollmentId,
    int StudentId,
    PaymentStatus Status,
    decimal Amount,
    string Currency,
    PaymentMethodType Method,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string? ExternalReference)
{
    public static string? MaskExternalReference(
    string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.Length <= 8)
            return "********";

        return $"{value[..4]}***{value[^4..]}";
    }
};