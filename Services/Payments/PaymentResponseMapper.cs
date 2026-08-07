using Cursos.Models.Payments;

namespace Cursos.Services.Payments;

public static class PaymentResponseMapper
{
    public static PaymentResponse ToResponse(
        Payment payment)
    {
        return new PaymentResponse(
            Id: payment.Id,
            EnrollmentId: payment.EnrollmentId,
            StudentId: payment.StudentId,
            Status: payment.Status,
            Amount: payment.Amount.Amount,
            Currency: payment.Amount.Currency,
            Method: payment.Method.Type,
            CreatedAt: payment.CreatedAt,
            UpdatedAt: payment.UpdatedAt,
            ExternalReference: MaskExternalReference(payment.IdempotencyKey));
    }

    private static string? MaskExternalReference(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.Length <= 8)
            return "********";

        return $"{value[..4]}***{value[^4..]}";
    }
}