namespace Cursos.Domain.Payments;

public sealed record PaymentStatusTransition
{
    public PaymentStatus From { get; }
    public PaymentStatus To { get; }
    public DateTimeOffset OccurredAt { get; }
    public string? Reason { get; }

    private PaymentStatusTransition(
        PaymentStatus from,
        PaymentStatus to,
        DateTimeOffset occurredAt,
        string? reason)
    {
        From = from;
        To = to;
        OccurredAt = occurredAt;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    public static PaymentStatusTransition Create(
        PaymentStatus from,
        PaymentStatus to,
        DateTimeOffset occurredAt,
        string? reason = null) =>
        new(from, to, occurredAt, reason);
}
