namespace Cursos.Domain.Payments;

public static class PaymentRules
{
    private static readonly Dictionary<PaymentStatus, List<PaymentStatus>> AllowedTransitions = new()
    {
        [PaymentStatus.Pending] = new List<PaymentStatus> 
        { 
            PaymentStatus.Confirmed, 
            PaymentStatus.Cancelled, 
            PaymentStatus.Failed 
        },
        [PaymentStatus.Confirmed] = new List<PaymentStatus> 
        { 
            PaymentStatus.Refunded 
        },
        [PaymentStatus.Cancelled] = new List<PaymentStatus>(),
        [PaymentStatus.Refunded] = new List<PaymentStatus>(),
        [PaymentStatus.Failed] = new List<PaymentStatus>()
    };
    
    public static bool CanTransitionTo(PaymentStatus from, PaymentStatus to)
    {
        return AllowedTransitions.TryGetValue(from, out var allowedStatuses) 
            && allowedStatuses.Contains(to);
    }
    
    public static IEnumerable<PaymentStatus> GetAllowedTransitions(PaymentStatus from)
    {
        return AllowedTransitions.TryGetValue(from, out var allowedStatuses) 
            ? allowedStatuses 
            : Enumerable.Empty<PaymentStatus>();
    }
    
    public static void ValidateTransition(PaymentStatus from, PaymentStatus to)
    {
        if (!CanTransitionTo(from, to))
        {
            throw new InvalidOperationException(
                $"Invalid payment status transition from {from} to {to}. " +
                $"Allowed transitions: {string.Join(", ", GetAllowedTransitions(from))}");
        }
    }
}
