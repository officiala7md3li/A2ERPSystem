using System.Collections.Generic;
using DomainDrivenERP.TaxEngine.Enums;

namespace DomainDrivenERP.TaxEngine.Interfaces;

public interface ITaxCalculationStrategy
{
    TaxTypeEnum TaxCode { get; }
    
    /// <summary>
    /// Computes the tax amount based on the base price and other already calculated taxes.
    /// </summary>
    decimal Calculate(decimal baseAmount, IReadOnlyDictionary<TaxTypeEnum, decimal> previouslyCalculatedTaxes);
    
    /// <summary>
    /// Returns the tax codes that MUST be calculated before this tax.
    /// </summary>
    IEnumerable<TaxTypeEnum> GetDependencies();
}
