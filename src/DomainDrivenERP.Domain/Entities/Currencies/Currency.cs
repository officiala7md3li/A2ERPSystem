using DomainDrivenERP.Domain.Entities.Currencies.DomainEvents;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Currencies;

/// <summary>
/// Represents a currency used across invoices, payments, and price lists.
/// ISO 4217 compliant (3-letter code).
/// </summary>
public sealed class Currency : AggregateRoot, IAuditableEntity
{
    private Currency() { }

    private Currency(Guid id, string code, string nameEn, string nameAr, string symbol, bool isBase)
        : base(id)
    {
        Code = code;
        NameEn = nameEn;
        NameAr = nameAr;
        Symbol = symbol;
        IsBase = isBase;
        IsActive = true;
    }

    public string Code { get; private set; }       // EGP, USD, EUR
    public string NameEn { get; private set; }     // Egyptian Pound
    public string NameAr { get; private set; }     // الجنيه المصري
    public string Symbol { get; private set; }     // ج.م
    public bool IsBase { get; private set; }       // العملة الأساسية للشركة
    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Result<Currency> Create(
        string code, string nameEn, string nameAr, string symbol, bool isBase = false)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
            return Result.Failure<Currency>(new Error("Currency.InvalidCode", "Currency code must be 3 letters (ISO 4217)."));

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure<Currency>(new Error("Currency.InvalidName", "English name is required."));

        if (string.IsNullOrWhiteSpace(symbol))
            return Result.Failure<Currency>(new Error("Currency.InvalidSymbol", "Symbol is required."));

        var currency = new Currency(Guid.NewGuid(), code.ToUpperInvariant(), nameEn, nameAr, symbol, isBase);
        currency.RaiseDomainEvent(new CurrencyCreatedDomainEvent(currency.Id));
        return Result.Success(currency);
    }

    public Result Activate()
    {
        IsActive = true;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (IsBase)
            return Result.Failure(new Error("Currency.CannotDeactivateBase", "Cannot deactivate the base currency."));
        IsActive = false;
        return Result.Success();
    }

    public Result SetAsBase()
    {
        IsBase = true;
        return Result.Success();
    }
}
