using DomainDrivenERP.TaxEngine.Enums;
using DomainDrivenERP.TaxEngine.Factories;
using DomainDrivenERP.TaxEngine.MetaData;
using DomainDrivenERP.TaxEngine.Services;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Engines;

public class TaxCalculationEngineTests
{
    // ─────────────────────────────────────────────────────────────
    // 1. Simple single-tax calculation
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxEngine_SingleRatioTax_ComputesCorrectAmount()
    {
        // Arrange
        var engine = new TaxCalculationEngineBuilder()
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(TaxTypeEnum.V009, 0.14m))
            .Build();

        // Act
        var results = engine.CalculateTaxes(1000m);

        // Assert
        results.Should().ContainKey(TaxTypeEnum.V009);
        results[TaxTypeEnum.V009].Should().Be(140m);
    }

    [Fact]
    public void TaxEngine_FixedTax_ComputesCorrectAmount()
    {
        // Arrange
        var engine = new TaxCalculationEngineBuilder()
            .WithStrategy(TaxStrategyFactory.CreateFixedStrategy(TaxTypeEnum.ST02, 50m))
            .Build();

        // Act
        var results = engine.CalculateTaxes(1000m);

        // Assert
        results[TaxTypeEnum.ST02].Should().Be(50m);
    }

    // ─────────────────────────────────────────────────────────────
    // 2. Withholding taxes return negative amounts
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxEngine_WithholdingTax_ReturnsNegativeAmount()
    {
        // Arrange — W001 is a W-code (withholding)
        var engine = new TaxCalculationEngineBuilder()
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(TaxTypeEnum.W001, 0.05m))
            .Build();

        // Act
        var results = engine.CalculateTaxes(1000m);

        // Assert: withholding should produce a deduction (negative)
        results[TaxTypeEnum.W001].Should().Be(-50m);
    }

    // ─────────────────────────────────────────────────────────────
    // 3. Compound VAT compounds on top of its dependency
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxEngine_CompoundVat_ComputesOnBaseAndTableTax()
    {
        // Arrange:  T2 (Tbl01 @ 5%) first, then VAT (V009 @ 14%) on Base + Tbl01
        var engine = new TaxCalculationEngineBuilder()
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(TaxTypeEnum.Tbl01, 0.05m))
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(
                TaxTypeEnum.V009, 0.14m,
                dependencies: new[] { TaxTypeEnum.Tbl01 }))
            .Build();

        // Act
        var results = engine.CalculateTaxes(1000m);

        // Tbl01 = 1000 * 5% = 50
        // V009  = (1000 + 50) * 14% = 1050 * 14% = 147
        results[TaxTypeEnum.Tbl01].Should().Be(50m);
        results[TaxTypeEnum.V009].Should().Be(147m);
    }

    // ─────────────────────────────────────────────────────────────
    // 4. Topological sort — T4 computed before T1
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxEngine_TopologicalSort_ProcessesDependenciesFirst()
    {
        // Define VAT depending on Tbl01 — even if added to the builder in reverse order
        var engine = new TaxCalculationEngineBuilder()
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(
                TaxTypeEnum.V009, 0.14m,
                dependencies: new[] { TaxTypeEnum.Tbl01 }))   // added FIRST
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(TaxTypeEnum.Tbl01, 0.05m))  // added SECOND
            .Build();

        // Act
        var results = engine.CalculateTaxes(1000m);

        // Tbl01 must be computed before V009 regardless of insertion order
        results[TaxTypeEnum.Tbl01].Should().Be(50m,  "Tbl01 = 1000 * 5%");
        results[TaxTypeEnum.V009].Should().Be(147m, "V009 = (1000+50) * 14%");
    }

    // ─────────────────────────────────────────────────────────────
    // 5. Circular dependency throws
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxEngine_CircularDependency_ThrowsInvalidOperation()
    {
        // A → B → A is a cycle
        var builder = new TaxCalculationEngineBuilder()
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(
                TaxTypeEnum.V009, 0.14m, dependencies: new[] { TaxTypeEnum.Tbl01 }))
            .WithStrategy(TaxStrategyFactory.CreateRatioStrategy(
                TaxTypeEnum.Tbl01, 0.05m, dependencies: new[] { TaxTypeEnum.V009 }));

        // Act & Assert
        var act = () => builder.Build();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Circular dependency*");
    }

    // ─────────────────────────────────────────────────────────────
    // 6. IsWCode extension method
    // ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(TaxTypeEnum.W001, true)]
    [InlineData(TaxTypeEnum.W016, true)]
    [InlineData(TaxTypeEnum.V009, false)]
    [InlineData(TaxTypeEnum.ST01, false)]
    public void TaxCategorizer_IsWCode_IdentifiesWithholdingCodes(TaxTypeEnum code, bool expected)
    {
        code.IsWCode().Should().Be(expected);
    }
}
