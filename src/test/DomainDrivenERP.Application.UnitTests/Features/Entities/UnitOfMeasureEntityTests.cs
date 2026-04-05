using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class UnitOfMeasureEntityTests
{
    // ─────────────────────────────────────────────────────────────
    // 1. Create — valid data
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void UoM_Create_WithValidData_Succeeds()
    {
        var result = UnitOfMeasure.Create("PCS", "Piece", "قطعة", UomType.Quantity);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("PCS");
        result.Value.NameEn.Should().Be("Piece");
        result.Value.NameAr.Should().Be("قطعة");
        result.Value.Type.Should().Be(UomType.Quantity);
        result.Value.ConversionFactor.Should().Be(1);
        result.Value.BaseUomId.Should().BeNull();
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UoM_Create_WithConversionFactor_Succeeds()
    {
        var baseId = Guid.NewGuid();
        var result = UnitOfMeasure.Create("BOX", "Box", "كرتونة", UomType.Quantity, 12, baseId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("BOX");
        result.Value.ConversionFactor.Should().Be(12);
        result.Value.BaseUomId.Should().Be(baseId);
    }

    [Fact]
    public void UoM_Create_CodeIsUppercased()
    {
        var result = UnitOfMeasure.Create("kg", "Kilogram", "كيلو", UomType.Weight);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("KG");
    }

    // ─────────────────────────────────────────────────────────────
    // 2. Create — validation failures
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void UoM_Create_WithEmptyCode_Fails()
    {
        var result = UnitOfMeasure.Create("", "Piece", "", UomType.Quantity);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("UoM.InvalidCode");
    }

    [Fact]
    public void UoM_Create_WithEmptyName_Fails()
    {
        var result = UnitOfMeasure.Create("PCS", "", "", UomType.Quantity);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("UoM.InvalidName");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UoM_Create_WithInvalidFactor_Fails(decimal factor)
    {
        var result = UnitOfMeasure.Create("BOX", "Box", "", UomType.Quantity, factor);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("UoM.InvalidFactor");
    }

    // ─────────────────────────────────────────────────────────────
    // 3. Conversion helpers
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void UoM_ToBase_ConvertsCorrectly()
    {
        // 1 BOX = 12 PCS
        var box = UnitOfMeasure.Create("BOX", "Box", "", UomType.Quantity, 12).Value;

        box.ToBase(3).Should().Be(36); // 3 BOX → 36 PCS
    }

    [Fact]
    public void UoM_FromBase_ConvertsCorrectly()
    {
        // 1 BOX = 12 PCS
        var box = UnitOfMeasure.Create("BOX", "Box", "", UomType.Quantity, 12).Value;

        box.FromBase(36).Should().Be(3); // 36 PCS → 3 BOX
    }

    [Fact]
    public void UoM_BaseUnit_ConversionIsIdentity()
    {
        var pcs = UnitOfMeasure.Create("PCS", "Piece", "", UomType.Quantity, 1).Value;

        pcs.ToBase(10).Should().Be(10);
        pcs.FromBase(10).Should().Be(10);
    }

    // ─────────────────────────────────────────────────────────────
    // 4. All UomType values
    // ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(UomType.Quantity)]
    [InlineData(UomType.Weight)]
    [InlineData(UomType.Volume)]
    [InlineData(UomType.Length)]
    [InlineData(UomType.Area)]
    [InlineData(UomType.Time)]
    public void UoM_Create_AllTypesSupported(UomType type)
    {
        var result = UnitOfMeasure.Create("TST", "Test", "", type);
        result.IsSuccess.Should().BeTrue();
        result.Value.Type.Should().Be(type);
    }
}
