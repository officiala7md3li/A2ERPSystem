using System.Collections.Generic;
using DomainDrivenERP.TaxEngine.Enums;
using DomainDrivenERP.TaxEngine.Interfaces;
using DomainDrivenERP.TaxEngine.Strategies;
using DomainDrivenERP.TaxEngine.MetaData;

namespace DomainDrivenERP.TaxEngine.Factories;

public static class TaxStrategyFactory
{
    /// <summary>
    /// Factory method to easily build tax strategies natively. In a full production scenario with DB-driven configs,
    /// this parses a TaxDefinition entity into a strategy.
    /// </summary>
    public static ITaxCalculationStrategy CreateRatioStrategy(
        TaxTypeEnum taxCode, 
        decimal rate, 
        IEnumerable<TaxTypeEnum> dependencies = null)
    {
        return new RatioTaxStrategy(taxCode, rate, dependencies, taxCode.IsWCode());
    }

    public static ITaxCalculationStrategy CreateFixedStrategy(
        TaxTypeEnum taxCode, 
        decimal amount)
    {
        return new FixedTaxStrategy(taxCode, amount, taxCode.IsWCode());
    }
}
