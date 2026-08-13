using System.Collections.Generic;
using Cursos.Domain.Common;
using Cursos.Domain.Exceptions;

namespace Cursos.Domain.Payments;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    public Money(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");
        
        Amount = amount;
        Currency = currency;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
    
    public static Money Zero(string currency = "BRL") => new(0, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Cannot add money with different currencies");
        return new Money(Amount + other.Amount, Currency);
    }
}
