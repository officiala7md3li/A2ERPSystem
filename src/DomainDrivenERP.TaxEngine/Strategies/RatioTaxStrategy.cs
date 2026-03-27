using System.Collections.Generic;
using System.Linq;
using DomainDrivenERP.TaxEngine.Enums;
using DomainDrivenERP.TaxEngine.Interfaces;

namespace DomainDrivenERP.TaxEngine.Strategies;

public sealed class RatioTaxStrategy : ITaxCalculationStrategy
{
    private readonly decimal _rate;
    private readonly HashSet<TaxTypeEnum> _dependencies;
    private readonly bool _isWithholding;

    public TaxTypeEnum TaxCode { get; }

    public RatioTaxStrategy(TaxTypeEnum taxCode, decimal rate, IEnumerable<TaxTypeEnum> dependencies = null, bool isWithholding = false)
    {
        TaxCode = taxCode;
        _rate = rate;
        _dependencies = new HashSet<TaxTypeEnum>(dependencies ?? Enumerable.Empty<TaxTypeEnum>());
        _isWithholding = isWithholding;
    }

    public decimal Calculate(decimal baseAmount, IReadOnlyDictionary<TaxTypeEnum, decimal> previouslyCalculatedTaxes)
    {
        // For compound taxes, sum the base amount + the requested dependent taxes
        decimal taxableAmount = baseAmount;

        foreach (var dep in _dependencies)
        {
            if (previouslyCalculatedTaxes.TryGetValue(dep, out var amount))
            {
                // We add the computed tax amount to the base to compound it.
                // Note: Withholding taxes (amount < 0) intrinsically reduce the taxable amount if they are a dependency.
                taxableAmount += amount;
            }
        }

        var calculatedTax = taxableAmount * _rate;
        return _isWithholding ? -Math.Abs(calculatedTax) : calculatedTax;
    }

    public IEnumerable<TaxTypeEnum> GetDependencies() => _dependencies;
}
