using System;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Payments;

namespace Cursos.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByGatewayTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
}
