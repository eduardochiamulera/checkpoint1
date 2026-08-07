using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Cursos.Domains.Payments;

namespace Cursos.Services.Payments;

public sealed class SimulatedPaymentGateway : IPaymentGateway
{
    private readonly ConcurrentDictionary<string, GatewayCreateResult> _created = new();
    private readonly ConcurrentDictionary<string, GatewayOperationResult> _operations = new();

    public Task<GatewayCreateResult> CreateAsync(
        GatewayCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = _created.GetOrAdd(
            request.IdempotencyKey,
            _ => CreateDeterministicResult(request));

        return Task.FromResult(result);
    }

    public Task<GatewayOperationResult> ConfirmAsync(
        GatewayConfirmRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = $"confirm:{request.IdempotencyKey}";
        var result = _operations.GetOrAdd(
            key,
            _ => CreateOperationResult(
                request.ExternalPaymentId,
                request.IdempotencyKey));

        return Task.FromResult(result);
    }

    public Task<GatewayOperationResult> RefundAsync(
        GatewayRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = $"refund:{request.IdempotencyKey}";
        var result = _operations.GetOrAdd(
            key,
            _ => CreateOperationResult(
                request.ExternalPaymentId,
                request.IdempotencyKey));

        return Task.FromResult(result);
    }

    private static GatewayCreateResult CreateDeterministicResult(
        GatewayCreateRequest request)
    {
        var behavior = request.IdempotencyKey.ToLowerInvariant();

        if (behavior.Contains("timeout"))
        {
            return new GatewayCreateResult(
                GatewayOperationStatus.Timeout,
                null,
                null,
                DateTimeOffset.UtcNow,
                "SIMULATED_TIMEOUT",
                "O gateway simulado excedeu o tempo limite.");
        }

        if (behavior.Contains("decline") || behavior.Contains("declined"))
        {
            return new GatewayCreateResult(
                GatewayOperationStatus.Declined,
                null,
                null,
                DateTimeOffset.UtcNow,
                "SIMULATED_DECLINED",
                "O pagamento foi recusado pelo gateway simulado.");
        }

        if (behavior.Contains("fail") || behavior.Contains("error"))
        {
            return new GatewayCreateResult(
                GatewayOperationStatus.Failed,
                null,
                null,
                DateTimeOffset.UtcNow,
                "SIMULATED_FAILURE",
                "O gateway simulado falhou ao criar o pagamento.");
        }

        var externalId = $"sim_{CreateStableToken(request.IdempotencyKey)}";
        var receipt = $"receipt_{CreateStableToken(externalId)}";

        return new GatewayCreateResult(
            GatewayOperationStatus.Succeeded,
            externalId,
            receipt,
            DateTimeOffset.UtcNow,
            null,
            null);
    }

    private static GatewayOperationResult CreateOperationResult(
        string externalPaymentId,
        string idempotencyKey)
    {
        var behavior = idempotencyKey.ToLowerInvariant();

        if (behavior.Contains("timeout"))
        {
            return new GatewayOperationResult(
                GatewayOperationStatus.Timeout,
                externalPaymentId,
                null,
                DateTimeOffset.UtcNow,
                "SIMULATED_TIMEOUT",
                "O gateway simulado excedeu o tempo limite.");
        }

        if (behavior.Contains("fail") || behavior.Contains("error"))
        {
            return new GatewayOperationResult(
                GatewayOperationStatus.Failed,
                externalPaymentId,
                null,
                DateTimeOffset.UtcNow,
                "SIMULATED_FAILURE",
                "O gateway simulado falhou na operação.");
        }

        return new GatewayOperationResult(
            GatewayOperationStatus.Succeeded,
            externalPaymentId,
            $"receipt_{CreateStableToken(idempotencyKey)}",
            DateTimeOffset.UtcNow,
            null,
            null);
    }

    private static string CreateStableToken(string value)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(value));

        return Convert.ToHexString(hash)[..24].ToLowerInvariant();
    }
}