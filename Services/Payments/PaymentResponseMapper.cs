using Cursos.Models.Payments;

namespace Cursos.Services.Payments;

public static class PaymentResponseMapper
{
    public static PaymentResponse ToResponse(
    Payment payment,
    string? externalPaymentId)
    {
        return new PaymentResponse(
            payment.Id,
            payment.EnrollmentId,
            payment.StudentId,
            payment.Status,
            payment.Amount.Amount,
            payment.Amount.Currency,
            payment.Method.Type,
            payment.CreatedAt,
            payment.UpdatedAt,
            MaskExternalReference(externalPaymentId));
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