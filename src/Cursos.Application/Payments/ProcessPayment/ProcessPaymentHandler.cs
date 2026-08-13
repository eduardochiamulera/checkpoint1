using System;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Interfaces;
using Cursos.Domain.Payments;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cursos.Application.Payments.ProcessPayment;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, PaymentResultDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessPaymentHandler> _logger;
    
    public ProcessPaymentHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork,
        ILogger<ProcessPaymentHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<PaymentResultDto> Handle(
        ProcessPaymentCommand request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing payment for enrollment {EnrollmentId}, amount {Amount}",
            request.EnrollmentId,
            request.Amount);
        
        // Idempotency: check if payment already exists for this enrollment
        var existing = await _paymentRepository
            .GetByEnrollmentIdAsync(request.EnrollmentId, cancellationToken);
        
        if (existing is { Status: PaymentStatus.Confirmed })
        {
            _logger.LogWarning(
                "Payment already confirmed for enrollment {EnrollmentId}, transaction {TransactionId}",
                request.EnrollmentId,
                existing.GatewayTransactionId);
            
            return new PaymentResultDto(
                Success: true,
                PaymentId: existing.Id,
                TransactionId: existing.GatewayTransactionId,
                ErrorMessage: null);
        }
        
        // Create Payment (domain rule)
        var paymentMethodType = Enum.Parse<PaymentMethodType>(request.PaymentMethodType, true);
        var payment = new Payment(
            enrollmentId: request.EnrollmentId,
            amount: new Money(request.Amount),
            paymentMethodType: paymentMethodType);
        
        await _paymentRepository.AddAsync(payment, cancellationToken);
        
        _logger.LogDebug("Payment {PaymentId} created for enrollment {EnrollmentId}",
            payment.Id, request.EnrollmentId);
        
        // Process on gateway
        var gatewayResult = await _paymentGateway.ProcessAsync(payment, cancellationToken);
        
        if (!gatewayResult.Success)
        {
            _logger.LogError(
                "Payment gateway failed for payment {PaymentId}, enrollment {EnrollmentId}. Error: {Error}",
                payment.Id,
                request.EnrollmentId,
                gatewayResult.ErrorMessage);
            
            payment.Cancel();
            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            
            return new PaymentResultDto(
                Success: false,
                PaymentId: null,
                TransactionId: null,
                ErrorMessage: gatewayResult.ErrorMessage);
        }
        
        // Confirm payment
        payment.Confirm(gatewayResult.TransactionId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Payment {PaymentId} confirmed successfully for enrollment {EnrollmentId}, transaction {TransactionId}",
            payment.Id,
            request.EnrollmentId,
            gatewayResult.TransactionId);
        
        return new PaymentResultDto(
            Success: true,
            PaymentId: payment.Id,
            TransactionId: gatewayResult.TransactionId,
            ErrorMessage: null);
    }
}
