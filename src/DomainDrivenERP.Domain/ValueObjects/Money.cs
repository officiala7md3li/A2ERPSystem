using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return Result.Failure<Money>(new Error("Money.InvalidAmount", "Amount cannot be negative."));

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<Money>(new Error("Money.InvalidCurrency", "Currency cannot be empty."));

        if (currency.Length != 3)
            return Result.Failure<Money>(new Error("Money.InvalidCurrency", "Currency must be a 3-letter ISO code."));

        return Result.Success(new Money(amount, currency.ToUpperInvariant()));
    }

    public static Money Zero(string currency = "EGP") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add {Currency} and {other.Currency}.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract {Currency} and {other.Currency}.");
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    public Money ApplyPercentage(decimal percentage) => new(Amount * (percentage / 100), Currency);

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
