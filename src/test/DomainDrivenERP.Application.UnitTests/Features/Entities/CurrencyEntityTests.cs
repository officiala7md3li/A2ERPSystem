using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Domain.Primitives;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class CurrencyEntityTests
{
    // ─────────────────────────────────────────────────────────────
    // 1. Create — valid data
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Currency_Create_WithValidData_Succeeds()
    {
        var result = Currency.Create("EGP", "Egyptian Pound", "الجنيه المصري", "ج.م", isBase: true);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("EGP");
        result.Value.NameEn.Should().Be("Egyptian Pound");
        result.Value.NameAr.Should().Be("الجنيه المصري");
        result.Value.Symbol.Should().Be("ج.م");
        result.Value.IsBase.Should().BeTrue();
        result.Value.IsActive.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Currency_Create_CodeIsUppercased()
    {
        var result = Currency.Create("usd", "US Dollar", "", "$");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("USD");
    }

    // ─────────────────────────────────────────────────────────────
    // 2. Create — validation failures
    // ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("", "Currency.InvalidCode")]
    [InlineData("AB", "Currency.InvalidCode")]
    [InlineData("ABCD", "Currency.InvalidCode")]
    public void Currency_Create_WithInvalidCode_Fails(string code, string expectedError)
    {
        var result = Currency.Create(code, "Test", "", "$");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(expectedError);
    }

    [Fact]
    public void Currency_Create_WithEmptyName_Fails()
    {
        var result = Currency.Create("USD", "", "", "$");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Currency.InvalidName");
    }

    [Fact]
    public void Currency_Create_WithEmptySymbol_Fails()
    {
        var result = Currency.Create("USD", "US Dollar", "", "");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Currency.InvalidSymbol");
    }

    // ─────────────────────────────────────────────────────────────
    // 3. Activate / Deactivate
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Currency_Deactivate_WhenNotBase_Succeeds()
    {
        var currency = Currency.Create("USD", "US Dollar", "", "$", isBase: false).Value;

        var result = currency.Deactivate();

        result.IsSuccess.Should().BeTrue();
        currency.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Currency_Deactivate_WhenBase_Fails()
    {
        var currency = Currency.Create("EGP", "Egyptian Pound", "", "ج.م", isBase: true).Value;

        var result = currency.Deactivate();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Currency.CannotDeactivateBase");
    }

    [Fact]
    public void Currency_Activate_RestoresActiveState()
    {
        var currency = Currency.Create("USD", "US Dollar", "", "$", isBase: false).Value;
        currency.Deactivate();

        var result = currency.Activate();

        result.IsSuccess.Should().BeTrue();
        currency.IsActive.Should().BeTrue();
    }

    // ─────────────────────────────────────────────────────────────
    // 4. Domain Event
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void Currency_Create_RaisesDomainEvent()
    {
        var currency = Currency.Create("EUR", "Euro", "يورو", "€").Value;

        currency.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<DomainDrivenERP.Domain.Entities.Currencies.DomainEvents.CurrencyCreatedDomainEvent>();
    }
}
