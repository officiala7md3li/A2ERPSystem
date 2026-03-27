using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Engines.DiscountEngine.Models;

/// <summary>
/// Represents a resolved discount candidate from a single source (e.g. Item, Campaign, PromoCode)
/// </summary>
public sealed record DiscountCandidate(
    DiscountSource Source,
    DiscountType Type,
    decimal Value,
    bool IsCombinable,
    DateTime? StartDate = null,
    DateTime? EndDate = null);

/// <summary>
/// The final resolved discount amount and its winning source.
/// </summary>
public sealed record ResolvedLineDiscount(
    decimal DiscountAmount,
    DiscountSource WinningSource,
    IReadOnlyList<DiscountCandidate> AppliedCandidates);

/// <summary>
/// A breakdown item recorded per-line on the invoice.
/// </summary>
public sealed record DiscountBreakdownItem(
    DiscountSource Source,
    DiscountType Type,
    decimal DiscountAmount);
