using System;
using Cursos.Domain.Common;

namespace Cursos.Domain.Payments;

public class PaymentStatusTransition : Entity
{
    public Guid PaymentId { get; private set; }
    public string FromStatus { get; private set; } = string.Empty;
    public string ToStatus { get; private set; } = string.Empty;
    public DateTime TransitionedAt { get; private set; }
    public string? Reason { get; private set; }
    public Guid? UserId { get; private set; }
    
    public PaymentStatusTransition(
        Guid paymentId,
        string fromStatus,
        string toStatus,
        string? reason = null,
        Guid? userId = null)
    {
        PaymentId = paymentId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        TransitionedAt = DateTime.UtcNow;
        Reason = reason;
        UserId = userId;
    }
    
    public static PaymentStatusTransition Create(
        Guid paymentId,
        PaymentStatus fromStatus,
        PaymentStatus toStatus,
        string? reason = null,
        Guid? userId = null)
    {
        return new PaymentStatusTransition(
            paymentId,
            fromStatus.ToString(),
            toStatus.ToString(),
            reason,
            userId);
    }
}
