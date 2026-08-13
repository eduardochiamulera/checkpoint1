using System;
using System.Threading;
using System.Threading.Tasks;
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
        // Simulate network delay
        await Task.Delay(100, cancellationToken);
        
        // Simulate 90% success rate
        var success = Random.Shared.NextDouble() > 0.1;
        
        if (!success)
        {
            _logger.LogWarning("Payment {PaymentId} failed simulation", payment.Id);
            return new PaymentResult(
                Success: false,
                TransactionId: null,
                ErrorMessage: "Simulated gateway failure");
        }
        
        var transactionId = $"sim_{Guid.NewGuid():N}";
        _logger.LogInformation("Payment {PaymentId} processed with transaction {TransactionId}", 
            payment.Id, transactionId);
        
        return new PaymentResult(
            Success: true,
            TransactionId: transactionId,
            ErrorMessage: null);
    }
    
    public async Task<RefundResult> RefundAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);
        return new RefundResult(Success: true, ErrorMessage: null);
    }
}
