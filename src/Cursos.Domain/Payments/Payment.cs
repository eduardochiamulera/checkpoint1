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
    
    public Payment(Guid enrollmentId, Money amount, PaymentMethodType paymentMethodType = PaymentMethodType.CreditCard)
    {
        EnrollmentId = enrollmentId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        PaymentMethodType = paymentMethodType;
    }
    
    public void Confirm(string gatewayTransactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Only pending payments can be confirmed. Current status: {Status}");
        
        Status = PaymentStatus.Confirmed;
        GatewayTransactionId = gatewayTransactionId;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Cancel()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Only pending payments can be cancelled. Current status: {Status}");
        
        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Refund()
    {
        if (Status != PaymentStatus.Confirmed)
            throw new DomainException($"Only confirmed payments can be refunded. Current status: {Status}");
        
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
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
