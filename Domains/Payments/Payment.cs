using Cursos.Domain.Exceptions;
using Cursos.Domain.Payments;
using Cursos.Domains.Payments;

public sealed class Payment
{
    private readonly List<PaymentStatusTransition> _transitions = [];

    private Payment()
    {
        // Usado exclusivamente pelo EF Core.
        Amount = null!;
        Method = null!;
        IdempotencyKey = null!;
    }

    private Payment(
        Guid id,
        int enrollmentId,
        int studentId,
        Money amount,
        PaymentMethod method,
        string idempotencyKey,
        DateTimeOffset createdAt)
    {
        Id = id;
        EnrollmentId = enrollmentId;
        StudentId = studentId;
        Amount = amount;
        Method = method;
        IdempotencyKey = idempotencyKey;
        Status = PaymentStatus.Pending;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public int EnrollmentId { get; private set; }
    public int StudentId { get; private set; }
    public string UserId { get; private set; } = null!;
    public Money Amount { get; private set; } = null!;
    public PaymentMethod Method { get; private set; } = null!;
    public string IdempotencyKey { get; private set; } = null!;
    public PaymentStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<PaymentStatusTransition> Transitions =>
        _transitions.AsReadOnly();

    public static Payment Create(
        int enrollmentId,
        int studentId,
        Money amount,
        PaymentMethod method,
        string idempotencyKey,
        DateTimeOffset? now = null,
        Guid? id = null)
    {
        if (enrollmentId <= 0)
            throw new DomainException(
                "invalid_enrollment",
                "A matrícula é obrigatória.");

        if (studentId <= 0)
            throw new DomainException(
                "invalid_student",
                "O estudante é obrigatório.");

        ArgumentNullException.ThrowIfNull(amount);
        ArgumentNullException.ThrowIfNull(method);

        var normalizedKey = idempotencyKey?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedKey))
            throw new DomainException(
                "missing_idempotency_key",
                "A chave de idempotência é obrigatória.");

        if (normalizedKey.Length > PaymentRules.MaxIdempotencyKeyLength)
            throw new DomainException(
                "invalid_idempotency_key",
                "A chave de idempotência excede o limite permitido.");

        var timestamp = now ?? DateTimeOffset.UtcNow;

        return new Payment(
            id ?? Guid.NewGuid(),
            enrollmentId,
            studentId,
            amount,
            method,
            normalizedKey,
            timestamp);
    }

     public void Confirm(DateTimeOffset? now = null, string? reason = null) =>
        TransitionTo(PaymentStatus.Paid, now, reason ?? "Pagamento confirmado.");

    public void Fail(string reason, DateTimeOffset? now = null) =>
        TransitionTo(PaymentStatus.Failed, now, reason);

    public void Refund(string reason, DateTimeOffset? now = null)
    {
        if (Status != PaymentStatus.Paid)
            throw new DomainException(
                "invalid_payment_transition",
                "Somente um pagamento pago pode ser estornado.");

        TransitionTo(PaymentStatus.Refunded, now, reason);
    }

    private void TransitionTo(
        PaymentStatus target,
        DateTimeOffset? now,
        string? reason)
    {
        if (!PaymentRules.CanTransition(Status, target))
            throw new DomainException(
                "invalid_payment_transition",
                $"Não é permitido alterar o pagamento de {Status} para {target}.");

        if (target == PaymentStatus.Failed && string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                "failure_reason_required",
                "O motivo da falha do pagamento é obrigatório.");

        var timestamp = now ?? DateTimeOffset.UtcNow;
        _transitions.Add(PaymentStatusTransition.Create(Status, target, timestamp, reason));
        Status = target;
        UpdatedAt = timestamp;
    }
}