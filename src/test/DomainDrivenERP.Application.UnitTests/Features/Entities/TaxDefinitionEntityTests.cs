using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class TaxDefinitionEntityTests
{
    // ─────────────────────────────────────────────────────────────
    // 1. Create — valid data
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxDef_Create_Percentage_Succeeds()
    {
        var result = TaxDefinition.Create(
            "V009", "VAT 14%", "ضريبة القيمة المضافة",
            TaxCalculationMethod.Percentage, 0.14m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("V009");
        result.Value.Rate.Should().Be(0.14m);
        result.Value.CalculationMethod.Should().Be(TaxCalculationMethod.Percentage);
        result.Value.IsWithholding.Should().BeFalse();
        result.Value.AppliesTo.Should().Be(TaxAppliesTo.LineLevel);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void TaxDef_Create_FixedAmount_Succeeds()
    {
        var result = TaxDefinition.Create(
            "ST02", "Stamp 50 EGP", "دمغة ثابتة",
            TaxCalculationMethod.FixedAmount, 50m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Rate.Should().Be(50m);
        result.Value.CalculationMethod.Should().Be(TaxCalculationMethod.FixedAmount);
    }

    [Fact]
    public void TaxDef_Create_Withholding_Succeeds()
    {
        var result = TaxDefinition.Create(
            "W001", "Withholding 5%", "خصم ضريبي",
            TaxCalculationMethod.Percentage, 0.05m,
            isWithholding: true, appliesTo: TaxAppliesTo.InvoiceLevel);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsWithholding.Should().BeTrue();
        result.Value.AppliesTo.Should().Be(TaxAppliesTo.InvoiceLevel);
    }

    // ─────────────────────────────────────────────────────────────
    // 2. Create — validation failures
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxDef_Create_WithEmptyCode_Fails()
    {
        var result = TaxDefinition.Create("", "Test", "", TaxCalculationMethod.Percentage, 0.1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TaxDef.InvalidCode");
    }

    [Fact]
    public void TaxDef_Create_WithEmptyName_Fails()
    {
        var result = TaxDefinition.Create("V001", "", "", TaxCalculationMethod.Percentage, 0.1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TaxDef.InvalidName");
    }

    [Fact]
    public void TaxDef_Create_WithNegativeRate_Fails()
    {
        var result = TaxDefinition.Create("V001", "Test", "", TaxCalculationMethod.Percentage, -0.1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TaxDef.InvalidRate");
    }

    [Fact]
    public void TaxDef_Create_PercentageAbove1_Fails()
    {
        var result = TaxDefinition.Create("V001", "Test", "", TaxCalculationMethod.Percentage, 1.5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TaxDef.InvalidRate");
    }

    // ─────────────────────────────────────────────────────────────
    // 3. Dependency management
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxDef_AddDependency_Succeeds()
    {
        var vat = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;
        var tableTaxId = Guid.NewGuid();

        var result = vat.AddDependency(tableTaxId);

        result.IsSuccess.Should().BeTrue();
        vat.Dependencies.Should().ContainSingle()
            .Which.DependsOnTaxId.Should().Be(tableTaxId);
    }

    [Fact]
    public void TaxDef_AddDependency_SelfReference_Fails()
    {
        var vat = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;

        var result = vat.AddDependency(vat.Id);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TaxDef.CyclicDependency");
    }

    [Fact]
    public void TaxDef_AddDependency_Duplicate_Fails()
    {
        var vat = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;
        var taxId = Guid.NewGuid();
        vat.AddDependency(taxId);

        var result = vat.AddDependency(taxId);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TaxDef.DuplicateDependency");
    }

    [Fact]
    public void TaxDef_AddMultipleDependencies_AllTracked()
    {
        var vat = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;

        vat.AddDependency(Guid.NewGuid());
        vat.AddDependency(Guid.NewGuid());
        vat.AddDependency(Guid.NewGuid());

        vat.Dependencies.Should().HaveCount(3);
    }

    [Fact]
    public void TaxDef_RemoveDependency_RemovesCorrectOne()
    {
        var vat = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;
        var taxId1 = Guid.NewGuid();
        var taxId2 = Guid.NewGuid();
        vat.AddDependency(taxId1);
        vat.AddDependency(taxId2);

        vat.RemoveDependency(taxId1);

        vat.Dependencies.Should().ContainSingle()
            .Which.DependsOnTaxId.Should().Be(taxId2);
    }

    // ─────────────────────────────────────────────────────────────
    // 4. UpdateRate
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxDef_UpdateRate_WithValidRate_Succeeds()
    {
        var tax = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;

        var result = tax.UpdateRate(0.15m);

        result.IsSuccess.Should().BeTrue();
        tax.Rate.Should().Be(0.15m);
    }

    [Fact]
    public void TaxDef_UpdateRate_WithNegativeRate_Fails()
    {
        var tax = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;

        var result = tax.UpdateRate(-0.01m);

        result.IsFailure.Should().BeTrue();
    }

    // ─────────────────────────────────────────────────────────────
    // 5. Activate / Deactivate
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxDef_Deactivate_SetsInactive()
    {
        var tax = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;

        tax.Deactivate();

        tax.IsActive.Should().BeFalse();
    }

    [Fact]
    public void TaxDef_Activate_RestoresActive()
    {
        var tax = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;
        tax.Deactivate();

        tax.Activate();

        tax.IsActive.Should().BeTrue();
    }

    // ─────────────────────────────────────────────────────────────
    // 6. Domain Event
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void TaxDef_Create_RaisesDomainEvent()
    {
        var tax = TaxDefinition.Create("V009", "VAT", "", TaxCalculationMethod.Percentage, 0.14m).Value;

        tax.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<DomainDrivenERP.Domain.Entities.TaxDefinitions.DomainEvents.TaxDefinitionCreatedDomainEvent>();
    }
}
