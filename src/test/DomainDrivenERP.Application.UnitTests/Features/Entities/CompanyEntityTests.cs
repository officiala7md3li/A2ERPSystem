using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Domain.Enums;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class CompanyEntityTests
{
    private static Guid ValidCurrencyId => Guid.NewGuid();

    // ─────────────────────────────────────────────────────────────
    // 1. Create — valid data
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Company_Create_WithValidData_Succeeds()
    {
        var currencyId = ValidCurrencyId;
        var result = Company.Create(
            "Test Company", "شركة اختبار", "123456789", currencyId);

        result.IsSuccess.Should().BeTrue();
        result.Value.NameEn.Should().Be("Test Company");
        result.Value.NameAr.Should().Be("شركة اختبار");
        result.Value.TaxRegistrationNumber.Should().Be("123456789");
        result.Value.BaseCurrencyId.Should().Be(currencyId);
        result.Value.IsActive.Should().BeTrue();
        result.Value.DefaultTaxOrder.Should().Be(TaxOrderSetting.AfterDiscount);
        result.Value.DefaultStackingMode.Should().Be(StackingMode.NoStacking);
        result.Value.StockValuation.Should().Be(StockValuationMethod.Average);
    }

    [Fact]
    public void Company_Create_WithCustomDefaults_Succeeds()
    {
        var result = Company.Create(
            "Custom Co", "شركة", "TAX001", ValidCurrencyId,
            TaxOrderSetting.BeforeDiscount, StackingMode.FullStacking, StockValuationMethod.FIFO);

        result.IsSuccess.Should().BeTrue();
        result.Value.DefaultTaxOrder.Should().Be(TaxOrderSetting.BeforeDiscount);
        result.Value.DefaultStackingMode.Should().Be(StackingMode.FullStacking);
        result.Value.StockValuation.Should().Be(StockValuationMethod.FIFO);
    }

    // ─────────────────────────────────────────────────────────────
    // 2. Create — validation failures
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Company_Create_WithEmptyNameEn_Fails()
    {
        var result = Company.Create("", "عربي", "TAX001", ValidCurrencyId);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidName");
    }

    [Fact]
    public void Company_Create_WithEmptyNameAr_Fails()
    {
        var result = Company.Create("English", "", "TAX001", ValidCurrencyId);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidNameAr");
    }

    [Fact]
    public void Company_Create_WithEmptyTaxNumber_Fails()
    {
        var result = Company.Create("Test", "اختبار", "", ValidCurrencyId);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidTaxNumber");
    }

    [Fact]
    public void Company_Create_WithEmptyCurrency_Fails()
    {
        var result = Company.Create("Test", "اختبار", "TAX001", Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidCurrency");
    }

    // ─────────────────────────────────────────────────────────────
    // 3. UpdatePipelineDefaults
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Company_UpdatePipelineDefaults_ChangesSettings()
    {
        var company = Company.Create("Test", "اختبار", "TAX001", ValidCurrencyId).Value;

        var result = company.UpdatePipelineDefaults(TaxOrderSetting.BeforeDiscount, StackingMode.FullStacking);

        result.IsSuccess.Should().BeTrue();
        company.DefaultTaxOrder.Should().Be(TaxOrderSetting.BeforeDiscount);
        company.DefaultStackingMode.Should().Be(StackingMode.FullStacking);
    }

    // ─────────────────────────────────────────────────────────────
    // 4. UpdateDiscountCaps
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Company_UpdateDiscountCaps_WithValidValues_Succeeds()
    {
        var company = Company.Create("Test", "اختبار", "TAX001", ValidCurrencyId).Value;

        var result = company.UpdateDiscountCaps(25m, 5000m);

        result.IsSuccess.Should().BeTrue();
        company.MaxDiscountPercentPerLine.Should().Be(25m);
        company.MaxDiscountAmountPerInvoice.Should().Be(5000m);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Company_UpdateDiscountCaps_WithInvalidPercent_Fails(decimal percent)
    {
        var company = Company.Create("Test", "اختبار", "TAX001", ValidCurrencyId).Value;

        var result = company.UpdateDiscountCaps(percent, 5000m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidCap");
    }

    // ─────────────────────────────────────────────────────────────
    // 5. UpdateInventorySettings
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Company_UpdateInventorySettings_ChangesAll()
    {
        var company = Company.Create("Test", "اختبار", "TAX001", ValidCurrencyId).Value;

        company.UpdateInventorySettings(true, true, StockValuationMethod.LIFO);

        company.ReserveStockOnSalesOrder.Should().BeTrue();
        company.AllowNegativeStock.Should().BeTrue();
        company.StockValuation.Should().Be(StockValuationMethod.LIFO);
    }

    // ─────────────────────────────────────────────────────────────
    // 6. Domain Event
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Company_Create_RaisesDomainEvent()
    {
        var company = Company.Create("Test", "اختبار", "TAX001", ValidCurrencyId).Value;

        company.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<DomainDrivenERP.Domain.Entities.Companies.DomainEvents.CompanyCreatedDomainEvent>();
    }
}
