using System;
using System.Collections.Generic;
using System.Linq;
using DomainDrivenERP.TaxEngine.Enums;
using DomainDrivenERP.TaxEngine.Interfaces;

namespace DomainDrivenERP.TaxEngine.Services;

/// <summary>
/// A completely autonomous, offline Engine that accepts a base price, topologically sorts the active tax strategies based on their defined dependencies, and executes them in the strictly correct mathematical sequence.
/// </summary>
public sealed class TaxCalculationEngine
{
    private readonly IReadOnlyList<ITaxCalculationStrategy> _sortedStrategies;

    internal TaxCalculationEngine(IReadOnlyList<ITaxCalculationStrategy> sortedStrategies)
    {
        _sortedStrategies = sortedStrategies;
    }

    /// <summary>
    /// Executes the active tax strategies in topologically sorted order against the given base amount.
    /// </summary>
    /// <param name="baseAmount">The net amount before taxes.</param>
    /// <returns>A dictionary of the calculated tax amounts mapped by their respective TaxCode.</returns>
    public IReadOnlyDictionary<TaxTypeEnum, decimal> CalculateTaxes(decimal baseAmount)
    {
        var results = new Dictionary<TaxTypeEnum, decimal>();

        foreach (var strategy in _sortedStrategies)
        {
            var taxAmount = strategy.Calculate(baseAmount, results);
            results[strategy.TaxCode] = taxAmount;
        }

        return results;
    }
}
