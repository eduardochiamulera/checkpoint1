using System.Globalization;
using Cursos.Domain.Exceptions;

namespace Cursos.Domain.Payments;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        var normalizedCurrency = currency?.Trim().ToUpperInvariant();

        if (amount <= 0)
            throw new DomainException(
                "invalid_payment_amount",
                "O valor do pagamento deve ser maior que zero.");

        if (amount != decimal.Round(amount, 2, MidpointRounding.ToEven))
            throw new DomainException(
                "invalid_payment_amount_precision",
                "O valor do pagamento deve possuir no máximo duas casas decimais.");

        if (string.IsNullOrWhiteSpace(normalizedCurrency) ||
            normalizedCurrency.Length != 3 ||
            normalizedCurrency.Any(c => c is < 'A' or > 'Z'))
        {
            throw new DomainException(
                "invalid_currency",
                "A moeda deve ser um código ISO 4217 de três letras.");
        }

        return new Money(amount, normalizedCurrency);
    }

    public override string ToString() =>
        $"{Amount.ToString("F2", CultureInfo.InvariantCulture)} {Currency}";
}