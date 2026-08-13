using Cursos.Domain.Interfaces;
using Cursos.Domain.Payments;
using MediatR;

namespace Cursos.Application.Payments.ProcessPayment;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, PaymentResultDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;
    
    public ProcessPaymentHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<PaymentResultDto> Handle(
        ProcessPaymentCommand request, 
        CancellationToken cancellationToken)
    {
        // Idempotency: check if payment already exists for this enrollment
        var existing = await _paymentRepository
            .GetByEnrollmentIdAsync(request.EnrollmentId, cancellationToken);
        
        if (existing is { Status: PaymentStatus.Confirmed })
        {
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
        
        // Process on gateway
        var gatewayResult = await _paymentGateway.ProcessAsync(payment, cancellationToken);
        
        if (!gatewayResult.Success)
        {
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
        
        return new PaymentResultDto(
            Success: true,
            PaymentId: payment.Id,
            TransactionId: gatewayResult.TransactionId,
            ErrorMessage: null);
    }
}
