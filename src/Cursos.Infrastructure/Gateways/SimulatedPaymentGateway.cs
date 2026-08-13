using Cursos.Domain.Payments;
using Microsoft.Extensions.Logging;

namespace Cursos.Infrastructure.Gateways;

public class SimulatedPaymentGateway : IPaymentGateway
{
    private readonly ILogger<SimulatedPaymentGateway> _logger;
    
    public SimulatedPaymentGateway(ILogger<SimulatedPaymentGateway> logger)
    {
        _logger = logger;
    }
    
    public async Task<PaymentResult> ProcessAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing payment {PaymentId} for enrollment {EnrollmentId}, amount {Amount}",
            payment.Id,
            payment.EnrollmentId,
            payment.Amount.Amount);
        
        try
        {
            // Simulate network delay
            await Task.Delay(100, cancellationToken);
            
            // Simulate 90% success rate
            var success = Random.Shared.NextDouble() > 0.1;
            
            if (!success)
            {
                _logger.LogWarning(
                    "Payment {PaymentId} failed simulation - gateway returned error",
                    payment.Id);
                
                return new PaymentResult(
                    Success: false,
                    TransactionId: null,
                    ErrorMessage: "Simulated gateway failure");
            }
            
            var transactionId = $"sim_{Guid.NewGuid():N}";
            _logger.LogInformation(
                "Payment {PaymentId} processed successfully with transaction {TransactionId}",
                payment.Id, transactionId);
            
            return new PaymentResult(
                Success: true,
                TransactionId: transactionId,
                ErrorMessage: null);
        }
        catch (OperationCanceledException)
        {
            _logger.LogError(
                "Payment {PaymentId} processing cancelled",
                payment.Id);
            
            return new PaymentResult(
                Success: false,
                TransactionId: null,
                ErrorMessage: "Payment processing cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Payment {PaymentId} failed with unexpected error",
                payment.Id);
            
            return new PaymentResult(
                Success: false,
                TransactionId: null,
                ErrorMessage: "Unexpected error during payment processing");
        }
    }
    
    public async Task<RefundResult> RefundAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing refund for transaction {TransactionId}", transactionId);
        
        await Task.Delay(50, cancellationToken);
        
        _logger.LogInformation("Refund processed successfully for transaction {TransactionId}", transactionId);
        
        return new RefundResult(Success: true, ErrorMessage: null);
    }
}
