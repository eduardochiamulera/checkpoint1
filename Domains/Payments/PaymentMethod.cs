using Cursos.Domain.Exceptions;

namespace Cursos.Domain.Payments;

/// <summary>
/// Contém somente dados não sensíveis do método de pagamento.
/// Nunca armazenar número, CVV, validade ou trilha do cartão.
/// </summary>
public sealed record PaymentMethod
{
    public PaymentMethodType Type { get; }
    public string? Provider { get; }
    public string? ProviderPaymentId { get; }

    private PaymentMethod(
        PaymentMethodType type,
        string? provider,
        string? providerPaymentId)
    {
        Type = type;
        Provider = Normalize(provider);
        ProviderPaymentId = Normalize(providerPaymentId);
    }

    public static PaymentMethod Create(
        PaymentMethodType type,
        string? provider = null,
        string? providerPaymentId = null)
    {
        if (!Enum.IsDefined(type))
            throw new DomainException("invalid_payment_method", "Método de pagamento inválido.");

        return new PaymentMethod(type, provider, providerPaymentId);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}