using Cursos.Data;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Services.Payments;

public sealed class PaymentOwnershipService
{
    private readonly AppDbContext _dbContext;

    public PaymentOwnershipService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureOwnerOrAdminAsync(
        Guid paymentId,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var payment = await _dbContext.Payments
            .AsNoTracking()
            .SingleOrDefaultAsync(
                value => value.Id == paymentId,
                cancellationToken);

        if (payment is null)
        {
            throw new KeyNotFoundException(
                "Pagamento não encontrado.");
        }

        if (!isAdmin && payment.UserId != userId)
        {
            // Não revele se o pagamento existe para outro usuário.
            throw new UnauthorizedAccessException();
        }
    }
}