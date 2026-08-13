using Cursos.Domain.Common;

namespace Cursos.Domain.Payments;

public class PaymentGatewayTransaction : Entity
{
    public Guid PaymentId { get; private set; }
    public string GatewayType { get; private set; } = string.Empty;
    public string TransactionId { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "BRL";
    public string? RawResponse { get; private set; }
    public string? ErrorMessage { get; private set; }
    
    public PaymentGatewayTransaction(
        Guid paymentId,
        string gatewayType,
        string transactionId,
        string status,
        decimal amount,
        string currency = "BRL")
    {
        PaymentId = paymentId;
        GatewayType = gatewayType;
        TransactionId = transactionId;
        Status = status;
        Amount = amount;
        Currency = currency;
    }
    
    public void UpdateResponse(string rawResponse)
    {
        RawResponse = rawResponse;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void MarkAsFailed(string errorMessage)
    {
        Status = "Failed";
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }
}
