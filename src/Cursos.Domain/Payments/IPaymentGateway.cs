using System.Threading;
using System.Threading.Tasks;

namespace Cursos.Domain.Payments;

public interface IPaymentGateway
{
    Task<PaymentResult> ProcessAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<RefundResult> RefundAsync(string transactionId, CancellationToken cancellationToken = default);
}
