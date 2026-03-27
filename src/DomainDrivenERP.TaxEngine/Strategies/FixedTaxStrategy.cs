using System.Collections.Generic;
using System.Linq;
using DomainDrivenERP.TaxEngine.Enums;
using DomainDrivenERP.TaxEngine.Interfaces;

namespace DomainDrivenERP.TaxEngine.Strategies;

public sealed class FixedTaxStrategy : ITaxCalculationStrategy
{
    private readonly decimal _fixedAmount;
    private readonly bool _isWithholding;

    public TaxTypeEnum TaxCode { get; }

    public FixedTaxStrategy(TaxTypeEnum taxCode, decimal fixedAmount, bool isWithholding = false)
    {
        TaxCode = taxCode;
        _fixedAmount = fixedAmount;
        _isWithholding = isWithholding;
    }

    public decimal Calculate(decimal baseAmount, IReadOnlyDictionary<TaxTypeEnum, decimal> previouslyCalculatedTaxes)
    {
        return _isWithholding ? -_fixedAmount : _fixedAmount;
    }

    public IEnumerable<TaxTypeEnum> GetDependencies() => Enumerable.Empty<TaxTypeEnum>();
}
