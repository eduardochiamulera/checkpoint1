using Cursos.Models.Payments;

namespace Cursos.Services.Payments;

public interface IPaymentService
{
    Task<PaymentResponse> CreateAsync(
        CreatePaymentRequest request,
        string userId,
        string idempotencyKey,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<PaymentResponse> GetByIdAsync(
        Guid paymentId,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentResponse>> ListAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}