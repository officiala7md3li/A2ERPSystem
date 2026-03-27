using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.ValueObjects;

public sealed class Quantity : ValueObject
{
    public decimal Value { get; }
    public string Unit { get; } // PCS, KG, L, M, etc.

    private Quantity(decimal value, string unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Result<Quantity> Create(decimal value, string unit = "PCS")
    {
        if (value <= 0)
            return Result.Failure<Quantity>(new Error("Quantity.Invalid", "Quantity must be greater than zero."));

        if (string.IsNullOrWhiteSpace(unit))
            return Result.Failure<Quantity>(new Error("Quantity.InvalidUnit", "Unit cannot be empty."));

        return Result.Success(new Quantity(value, unit.ToUpperInvariant()));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
        yield return Unit;
    }

    public override string ToString() => $"{Value} {Unit}";
}
