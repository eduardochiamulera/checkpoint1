using Cursos.Domain.Exceptions;

namespace Cursos.Domains.Payments;

public sealed class Money
{
    private Money()
    {
        // Usado pelo EF Core.
        Currency = null!;
    }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = null!;

    public static Money Create(
        decimal amount,
        string currency)
    {
        var normalizedCurrency =
            currency?.Trim().ToUpperInvariant();

        if (amount <= 0)
        {
            throw new DomainException(
                "invalid_payment_amount",
                "O valor do pagamento deve ser maior que zero.");
        }

        if (amount != decimal.Round(
                amount,
                2,
                MidpointRounding.ToEven))
        {
            throw new DomainException(
                "invalid_payment_amount_precision",
                "O valor deve possuir no máximo duas casas decimais.");
        }

        if (string.IsNullOrWhiteSpace(normalizedCurrency) ||
            normalizedCurrency.Length != 3 ||
            normalizedCurrency.Any(
                character => character is < 'A' or > 'Z'))
        {
            throw new DomainException(
                "invalid_currency",
                "A moeda deve ser um código ISO 4217 de três letras.");
        }

        return new Money(
            decimal.Round(
                amount,
                2,
                MidpointRounding.ToEven),
            normalizedCurrency);
    }

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
}