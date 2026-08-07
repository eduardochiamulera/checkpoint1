namespace Cursos.Domain.Payments;

public static class PaymentRules
{
    public const int MaxIdempotencyKeyLength = 100;

    public static bool IsActive(PaymentStatus status) =>
        status is PaymentStatus.Pending or PaymentStatus.Paid;

    public static bool CanTransition(PaymentStatus from, PaymentStatus to) =>
        (from, to) switch
        {
            (PaymentStatus.Pending, PaymentStatus.Paid) => true,
            (PaymentStatus.Pending, PaymentStatus.Failed) => true,
            (PaymentStatus.Paid, PaymentStatus.Refunded) => true,
            _ => false
        };
}