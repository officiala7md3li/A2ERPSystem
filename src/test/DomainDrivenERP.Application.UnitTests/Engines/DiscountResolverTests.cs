using DomainDrivenERP.Application.Engines.DiscountEngine;
using DomainDrivenERP.Application.Engines.DiscountEngine.Models;
using DomainDrivenERP.Domain.Enums;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Engines;

public class DiscountResolverTests
{
    private readonly DiscountResolver _sut = new();

    private static DiscountCandidate MakeCandidate(
        DiscountSource source,
        DiscountType type = DiscountType.Percentage,
        decimal value = 10m,
        bool combinable = true,
        DateTime? start = null,
        DateTime? end = null)
        => new(source, type, value, combinable, start, end);

    // ─────────────────────────────────────────────────────────────
    // 1. No candidates → zero discount
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Resolve_NoCandidates_ReturnsZeroDiscount()
    {
        var result = _sut.Resolve([], 1000m, StackingMode.NoStacking);

        result.DiscountAmount.Should().Be(0);
        result.WinningSource.Should().Be(DiscountSource.None);
    }

    // ─────────────────────────────────────────────────────────────
    // 2. NoStacking → highest VALUE wins (not highest priority)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Resolve_NoStacking_HighestValueWins()
    {
        var candidates = new[]
        {
            MakeCandidate(DiscountSource.Category,       value: 5m),   // lower value
            MakeCandidate(DiscountSource.ManualOverride, value: 20m),  // higher source priority but check value logic
            MakeCandidate(DiscountSource.PriceList,      value: 25m),  // highest value
        };

        var result = _sut.Resolve(candidates, 1000m, StackingMode.NoStacking);

        // NoStacking picks highest-amount candidate
        result.DiscountAmount.Should().Be(250m, "25% of 1000 = 250");
        result.AppliedCandidates.Should().HaveCount(1);
    }

    // ─────────────────────────────────────────────────────────────
    // 3. FullStacking → all combine cumulatively on remaining amount
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Resolve_FullStacking_AllCandidatesCombine()
    {
        var candidates = new[]
        {
            MakeCandidate(DiscountSource.Category, value: 10m),   // 10% of 1000 = 100; remaining 900
            MakeCandidate(DiscountSource.PriceList, value: 10m),  // 10% of 900  = 90;  remaining 810
        };

        var result = _sut.Resolve(candidates, 1000m, StackingMode.FullStacking);

        result.DiscountAmount.Should().Be(190m, "100 + 90 = 190 (cascading)");
        result.AppliedCandidates.Should().HaveCount(2);
    }

    // ─────────────────────────────────────────────────────────────
    // 4. ConditionalStacking — non-combinable wins alone
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Resolve_ConditionalStacking_NonCombinableBlocksOthers()
    {
        var candidates = new[]
        {
            MakeCandidate(DiscountSource.Campaign,       value: 15m, combinable: false), // exclusive
            MakeCandidate(DiscountSource.PriceList,      value: 10m, combinable: true),
        };

        var result = _sut.Resolve(candidates, 1000m, StackingMode.ConditionalStacking);

        // Only the non-combinable discount is applied
        result.AppliedCandidates.Should().HaveCount(1);
        result.DiscountAmount.Should().Be(150m);
    }

    [Fact]
    public void Resolve_ConditionalStacking_AllCombinableStack()
    {
        var candidates = new[]
        {
            MakeCandidate(DiscountSource.Campaign,  value: 10m, combinable: true),
            MakeCandidate(DiscountSource.PriceList, value: 10m, combinable: true),
        };

        var result = _sut.Resolve(candidates, 1000m, StackingMode.ConditionalStacking);

        result.AppliedCandidates.Should().HaveCount(2);
        result.DiscountAmount.Should().Be(190m, "cascading: 100 + 90");
    }

    // ─────────────────────────────────────────────────────────────
    // 5. Per-line cap is enforced
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Resolve_PerLineCap_LimitsDiscount()
    {
        var candidates = new[]
        {
            MakeCandidate(DiscountSource.ManualOverride, value: 50m), // 50% of 1000 = 500
        };

        var result = _sut.Resolve(candidates, 1000m, StackingMode.NoStacking, perLineCapPercent: 20m);

        result.DiscountAmount.Should().Be(200m, "cap at 20% of 1000 = 200");
    }

    // ─────────────────────────────────────────────────────────────
    // 6. Expired seasonal discount is filtered out
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Resolve_ExpiredDiscount_IsIgnored()
    {
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var candidates = new[]
        {
            MakeCandidate(DiscountSource.Campaign, value: 20m, end: yesterday), // expired
        };

        var result = _sut.Resolve(candidates, 1000m, StackingMode.NoStacking);

        result.DiscountAmount.Should().Be(0);
        result.WinningSource.Should().Be(DiscountSource.None);
    }

    // ─────────────────────────────────────────────────────────────
    // 7. FixedAmount discount is capped at subtotal
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Resolve_FixedDiscount_CappedAtSubTotal()
    {
        var candidates = new[]
        {
            MakeCandidate(DiscountSource.ManualOverride, type: DiscountType.FixedAmount, value: 5000m),
        };

        var result = _sut.Resolve(candidates, 1000m, StackingMode.NoStacking);

        result.DiscountAmount.Should().Be(1000m, "cannot discount more than the subtotal");
    }
}
