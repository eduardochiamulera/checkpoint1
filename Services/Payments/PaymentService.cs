using Cursos.Data;
using Cursos.Domain.Exceptions;
using Cursos.Domain.Payments;
using Cursos.Domains;
using Cursos.Domains.Payments;
using Cursos.Models.Payments;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Services.Payments;

public sealed class PaymentService : IPaymentService
{
    private readonly AppDbContext _dbContext;
    private readonly IPaymentGateway _gateway;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        AppDbContext dbContext,
        IPaymentGateway gateway,
        ILogger<PaymentService> logger)
    {
        _dbContext = dbContext;
        _gateway = gateway;
        _logger = logger;
    }

    public async Task<PaymentResponse> CreateAsync(
        CreatePaymentRequest request,
        string userId,
        string idempotencyKey,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException();
        }

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new DomainException(
                "missing_idempotency_key",
                "O header Idempotency-Key é obrigatório.");
        }

        var existingPayment = await _dbContext.Payments
            .AsNoTracking()
            .SingleOrDefaultAsync(
                payment =>
                    payment.UserId == userId &&
                    payment.IdempotencyKey == idempotencyKey,
                cancellationToken);

        if (existingPayment is not null)
        {
            return ToResponse(existingPayment);
        }

        var enrollment = await _dbContext.Enrollments.SingleOrDefaultAsync(value => value.Id == request.EnrollmentId, cancellationToken);

        if (enrollment is null)
        {
            throw new KeyNotFoundException("Matrícula não encontrada.");
        }

        if (!isAdmin && enrollment.Student.UserId != userId)
        {
            throw new UnauthorizedAccessException();
        }

        if (enrollment.Status != EnrollmentStatus.Ativo)
        {
            throw new DomainException("inactive_enrollment","A matrícula não está ativa.");
        }

        var student = await _dbContext.Students.AsNoTracking().SingleOrDefaultAsync(
                value => value.UserId == userId &&
                         !value.IsDeleted,
                cancellationToken);

        if (student is null)
        {
            throw new DomainException(
                "student_not_found",
                "Nenhum estudante foi encontrado para o usuário autenticado.");
        }

        var hasActivePayment = await _dbContext.Payments
            .AsNoTracking()
            .AnyAsync(
                payment =>
                    payment.EnrollmentId == request.EnrollmentId &&
                    (payment.Status == PaymentStatus.Pending ||
                     payment.Status == PaymentStatus.Paid),
                cancellationToken);

        if (hasActivePayment)
        {
            throw new DomainException(
                "active_payment_exists",
                "Já existe um pagamento ativo para esta matrícula.");
        }

        var money = Money.Create(
            request.Amount,
            request.Currency);

        var method = PaymentMethod.Create(
            request.Method!.Value);

        var payment = Payment.Create(
            enrollmentId: enrollment.Id,
            studentId: student.Id,
            userId: userId,
            amount: money,
            method: method,
            idempotencyKey: idempotencyKey);

        _dbContext.Payments.Add(payment);

        var gatewayResult = await _gateway.CreateAsync(
            new GatewayCreateRequest(
                payment.Id,
                money.Amount,
                money.Currency,
                method.Type,
                idempotencyKey),
            cancellationToken);

        var externalId = gatewayResult.ExternalPaymentId
            ?? $"failed_{payment.Id:N}";

        var transaction = PaymentGatewayTransaction.Create(
            payment.Id,
            GatewayTransactionOperation.Create,
            externalId,
            gatewayResult.Receipt,
            gatewayResult.Status,
            gatewayResult.OccurredAt,
            gatewayResult.ErrorCode,
            gatewayResult.ErrorMessage);

        _dbContext.PaymentGatewayTransactions.Add(transaction);

        if (!gatewayResult.Succeeded)
        {
            payment.Fail(
                gatewayResult.ErrorMessage ??
                "O gateway não conseguiu criar o pagamento.");
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Pagamento criado. PaymentId: {PaymentId}, Status: {Status}",
            payment.Id,
            payment.Status);

        return ToResponse(payment);
    }

    public async Task<PaymentResponse> GetByIdAsync(
        Guid paymentId,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
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
            throw new UnauthorizedAccessException();
        }

        return ToResponse(payment);
    }

    public async Task<IReadOnlyList<PaymentResponse>> ListAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Payments
            .AsNoTracking();

        if (!isAdmin)
        {
            query = query.Where(
                payment => payment.UserId == userId);
        }

        var payments = await query
            .OrderByDescending(payment => payment.CreatedAt)
            .ToListAsync(cancellationToken);

        return payments
            .Select(ToResponse)
            .ToList();
    }

    private static PaymentResponse ToResponse(
        Payment payment)
    {
        return new PaymentResponse(
            payment.Id,
            payment.EnrollmentId,
            payment.StudentId,
            payment.Status,
            payment.Amount.Amount,
            payment.Amount.Currency,
            payment.Method.Type,
            payment.CreatedAt,
            payment.UpdatedAt,
            null);
    }
}