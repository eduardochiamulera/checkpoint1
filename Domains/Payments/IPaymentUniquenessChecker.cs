namespace Cursos.Domain.Payments;

public interface IPaymentUniquenessChecker
{
    Task<bool> HasActivePaymentAsync(
        Guid enrollmentId,
        CancellationToken cancellationToken = default);

    Task<Payment?> FindByIdempotencyKeyAsync(
        Guid studentId,
        string idempotencyKey,
        CancellationToken cancellationToken = default);
}