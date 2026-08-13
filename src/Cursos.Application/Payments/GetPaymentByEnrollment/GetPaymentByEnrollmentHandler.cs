using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Interfaces;
using Cursos.Domain.Payments;
using MediatR;

namespace Cursos.Application.Payments.GetPaymentByEnrollment;

public class GetPaymentByEnrollmentHandler : IRequestHandler<GetPaymentByEnrollmentQuery, PaymentDto?>
{
    private readonly IPaymentRepository _paymentRepository;
    
    public GetPaymentByEnrollmentHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }
    
    public async Task<PaymentDto?> Handle(
        GetPaymentByEnrollmentQuery request, 
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByEnrollmentIdAsync(
            request.EnrollmentId, cancellationToken);
        
        if (payment is null)
            return null;
        
        return new PaymentDto(
            Id: payment.Id,
            EnrollmentId: payment.EnrollmentId,
            Amount: payment.Amount.Amount,
            Currency: payment.Amount.Currency,
            Status: payment.Status.ToString(),
            TransactionId: payment.GatewayTransactionId,
            CreatedAt: payment.CreatedAt);
    }
}
