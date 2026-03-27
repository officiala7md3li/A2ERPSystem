using System;
using System.Collections.Generic;
using System.Linq;
using DomainDrivenERP.TaxEngine.Enums;
using DomainDrivenERP.TaxEngine.Interfaces;

namespace DomainDrivenERP.TaxEngine.Services;

/// <summary>
/// Auto-resolves the mathematical dependency graph of calculating Egyptian taxes and emits a finalized TaxCalculationEngine.
/// </summary>
public sealed class TaxCalculationEngineBuilder
{
    private readonly Dictionary<TaxTypeEnum, ITaxCalculationStrategy> _strategies = new();

    public TaxCalculationEngineBuilder WithStrategy(ITaxCalculationStrategy strategy)
    {
        // Add or overwrite
        _strategies[strategy.TaxCode] = strategy;
        return this;
    }

    public TaxCalculationEngine Build()
    {
        var sorted = TopologicalSort(_strategies.Values);
        return new TaxCalculationEngine(sorted);
    }

    /// <summary>
    /// Kahn's Algorithm for Topological Sorting to ensure dependent taxes are calculated first.
    /// </summary>
    private static IReadOnlyList<ITaxCalculationStrategy> TopologicalSort(IEnumerable<ITaxCalculationStrategy> strategies)
    {
        var activeStrategies = strategies.ToList();
        var activeCodes = new HashSet<TaxTypeEnum>(activeStrategies.Select(s => s.TaxCode));
        
        // Calculate in-degrees (number of dependencies that are ALSO active in this calculation)
        var inDegree = new Dictionary<TaxTypeEnum, int>();
        var adjacencyList = new Dictionary<TaxTypeEnum, List<TaxTypeEnum>>();

        foreach (var strategy in activeStrategies)
        {
            var code = strategy.TaxCode;
            inDegree[code] = 0;
            adjacencyList[code] = new List<TaxTypeEnum>();
        }

        foreach (var strategy in activeStrategies)
        {
            var u = strategy.TaxCode;
            
            // Only consider dependencies that are actually part of the current active strategies
            var activeDependencies = strategy.GetDependencies()
                .Where(dep => activeCodes.Contains(dep))
                .ToList();

            inDegree[u] = activeDependencies.Count;

            // directed edge: dependency -> strategy (v -> u)
            foreach (var v in activeDependencies)
            {
                adjacencyList[v].Add(u);
            }
        }

        var queue = new Queue<TaxTypeEnum>();
        foreach (var kvp in inDegree)
        {
            if (kvp.Value == 0)
            {
                queue.Enqueue(kvp.Key);
            }
        }

        var sortedCodes = new List<TaxTypeEnum>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sortedCodes.Add(current);

            foreach (var neighbor in adjacencyList[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (sortedCodes.Count != activeStrategies.Count)
        {
            throw new InvalidOperationException("Circular dependency detected in the tax calculation strategies.");
        }

        var strategyMap = activeStrategies.ToDictionary(s => s.TaxCode);
        return sortedCodes.Select(code => strategyMap[code]).ToList();
    }
}
