namespace Cursos.Domain.Payments;

public interface IPaymentUniquenessChecker
{
    Task<bool> HasActivePaymentAsync(
        int enrollmentId,
        CancellationToken cancellationToken = default);

    Task<Payment?> FindByIdempotencyKeyAsync(
        Guid studentId,
        string idempotencyKey,
        CancellationToken cancellationToken = default);
}