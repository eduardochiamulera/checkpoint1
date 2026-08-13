using Cursos.Domain.Common;
using Cursos.Domain.Exceptions;

namespace Cursos.Domain.Payments;

public class Payment : Entity
{
    public Guid EnrollmentId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public PaymentMethodType PaymentMethodType { get; private set; }
    
    private readonly List<PaymentStatusTransition> _statusTransitions = new();
    public IReadOnlyCollection<PaymentStatusTransition> StatusTransitions => _statusTransitions.AsReadOnly();
    
    public Payment(Guid enrollmentId, Money amount, PaymentMethodType paymentMethodType = PaymentMethodType.CreditCard)
    {
        EnrollmentId = enrollmentId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        PaymentMethodType = paymentMethodType;
        
        // Register initial transition
        _statusTransitions.Add(PaymentStatusTransition.Create(
            Id, 
            PaymentStatus.Pending, 
            PaymentStatus.Pending, 
            "Payment created"));
    }
    
    public void Confirm(string gatewayTransactionId)
    {
        PaymentRules.ValidateTransition(Status, PaymentStatus.Confirmed);
        
        var oldStatus = Status;
        Status = PaymentStatus.Confirmed;
        GatewayTransactionId = gatewayTransactionId;
        UpdatedAt = DateTime.UtcNow;
        
        _statusTransitions.Add(PaymentStatusTransition.Create(
            Id,
            oldStatus,
            PaymentStatus.Confirmed,
            "Payment confirmed by gateway"));
    }
    
    public void Cancel(string? reason = null)
    {
        PaymentRules.ValidateTransition(Status, PaymentStatus.Cancelled);
        
        var oldStatus = Status;
        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        
        _statusTransitions.Add(PaymentStatusTransition.Create(
            Id,
            oldStatus,
            PaymentStatus.Cancelled,
            reason ?? "Payment cancelled"));
    }
    
    public void Refund(string? reason = null)
    {
        PaymentRules.ValidateTransition(Status, PaymentStatus.Refunded);
        
        var oldStatus = Status;
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
        
        _statusTransitions.Add(PaymentStatusTransition.Create(
            Id,
            oldStatus,
            PaymentStatus.Refunded,
            reason ?? "Payment refunded"));
    }
    
    public void MarkAsFailed(string errorMessage)
    {
        PaymentRules.ValidateTransition(Status, PaymentStatus.Failed);
        
        var oldStatus = Status;
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
        
        _statusTransitions.Add(PaymentStatusTransition.Create(
            Id,
            oldStatus,
            PaymentStatus.Failed,
            errorMessage));
    }
}

public enum PaymentStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Refunded,
    Failed
}

public enum PaymentMethodType
{
    CreditCard,
    DebitCard,
    Pix,
    BankTransfer
}
