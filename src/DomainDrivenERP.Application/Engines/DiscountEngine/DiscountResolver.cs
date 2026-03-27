using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Application.Engines.DiscountEngine.Models;

namespace DomainDrivenERP.Application.Engines.DiscountEngine;

/// <summary>
/// Resolves multiple discount candidates from competing sources into a single final discount amount 
/// based on the Company-configured StackingMode, applying per-line cap rules.
/// </summary>
public sealed class DiscountResolver
{
    // Source priority order (highest = most important, applied first / wins in NoStacking)
    private static readonly Dictionary<DiscountSource, int> Priority = new()
    {
        [DiscountSource.ManualOverride] = 7,
        [DiscountSource.Campaign]       = 6,
        [DiscountSource.PriceList]      = 5,
        [DiscountSource.Loyalty]        = 4,
        [DiscountSource.PromoCode]      = 3,
        [DiscountSource.Item]           = 2,
        [DiscountSource.Category]       = 1,
        [DiscountSource.None]           = 0
    };

    /// <summary>
    /// Resolves the effective discount from all candidates for a single invoice line.
    /// </summary>
    /// <param name="candidates">All discount candidates resolved for this line.</param>
    /// <param name="lineSubTotal">The SubTotal of the line (Qty × UnitPrice).</param>
    /// <param name="stackingMode">The company-configured stacking mode.</param>
    /// <param name="perLineCapPercent">Optional maximum discount % allowed per line (0 = no cap).</param>
    public ResolvedLineDiscount Resolve(
        IEnumerable<DiscountCandidate> candidates,
        decimal lineSubTotal,
        StackingMode stackingMode,
        decimal perLineCapPercent = 0)
    {
        DateTime now = DateTime.UtcNow;

        // 1. Filter out expired or inactive seasonal discounts
        var validCandidates = candidates
            .Where(c => IsActive(c, now))
            .OrderByDescending(c => Priority.GetValueOrDefault(c.Source))
            .ToList();

        if (validCandidates.Count == 0)
        {
            return new ResolvedLineDiscount(0, DiscountSource.None, []);
        }

        List<DiscountCandidate> appliedCandidates;

        switch (stackingMode)
        {
            case StackingMode.NoStacking:
                // Only the highest-value discount wins
                (DiscountCandidate Candidate, decimal Amount) best = validCandidates
                    .Select(c => (Candidate: c, Amount: ComputeAmount(c, lineSubTotal)))
                    .OrderByDescending(x => x.Amount)
                    .First();
                appliedCandidates = [best.Candidate];
                break;

            case StackingMode.FullStacking:
                // All discounts combine cumulatively
                appliedCandidates = validCandidates;
                break;

            case StackingMode.ConditionalStacking:
                // If there is a non-combinable candidate, use highest-priority non-combinable only
                DiscountCandidate? nonCombinable = validCandidates.FirstOrDefault(c => !c.IsCombinable);
                appliedCandidates = nonCombinable is not null ? [nonCombinable] : validCandidates.Where(c => c.IsCombinable).ToList();
                break;

            default:
                appliedCandidates = [];
                break;
        }

        // 2. Compute actual cumulative discount amount against lineSubTotal
        decimal remaining = lineSubTotal;
        decimal totalDiscount = 0;
        foreach (DiscountCandidate candidate in appliedCandidates)
        {
            decimal amount = ComputeAmount(candidate, remaining);
            totalDiscount += amount;
            remaining = Math.Max(0, remaining - amount);
        }

        // 3. Apply per-line cap
        if (perLineCapPercent > 0)
        {
            decimal maxAllowed = lineSubTotal * perLineCapPercent / 100m;
            totalDiscount = Math.Min(totalDiscount, maxAllowed);
        }

        DiscountSource winningSource = appliedCandidates.Count > 0
            ? appliedCandidates.OrderByDescending(c => Priority.GetValueOrDefault(c.Source)).First().Source
            : DiscountSource.None;

        return new ResolvedLineDiscount(totalDiscount, winningSource, appliedCandidates);
    }

    private static decimal ComputeAmount(DiscountCandidate candidate, decimal subTotal)
    {
        return candidate.Type switch
        {
            DiscountType.FixedAmount => Math.Min(candidate.Value, subTotal),
            DiscountType.Percentage => subTotal * candidate.Value / 100m,
            DiscountType.Seasonal => subTotal * candidate.Value / 100m,
            DiscountType.Tiered => subTotal * candidate.Value / 100m, // Tiered logic resolved externally before passing in
            _ => 0
        };
    }

    private static bool IsActive(DiscountCandidate candidate, DateTime now)
    {
        if (candidate.StartDate.HasValue && now < candidate.StartDate.Value) return false;
        if (candidate.EndDate.HasValue && now > candidate.EndDate.Value) return false;
        return true;
    }
}
