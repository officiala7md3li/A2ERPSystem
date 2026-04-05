using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Primitives;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

/// <summary>
/// Tests verifying the Phase 1 bug fixes applied to InvoiceLine.
/// </summary>
public class InvoiceLineBugFixTests
{
    // ─────────────────────────────────────────────────────────────
    // Fix 1: InvoiceLine inherits BaseEntity, not AggregateRoot
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void InvoiceLine_IsBaseEntity_NotAggregateRoot()
    {
        typeof(InvoiceLine).BaseType.Should().Be(typeof(BaseEntity),
            "InvoiceLine is a child entity inside CustomerInvoice aggregate — it must not be an AggregateRoot");
    }

    [Fact]
    public void InvoiceLine_DoesNotImplement_IAuditableEntity()
    {
        typeof(InvoiceLine).Should().NotImplement<IAuditableEntity>(
            "InvoiceLine is managed by its parent aggregate and doesn't need its own auditing");
    }

    // ─────────────────────────────────────────────────────────────
    // Fix 2: NotImplementedException removed
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void InvoiceLine_DoesNotHave_CreatedOnUtc_Property()
    {
        var props = typeof(InvoiceLine).GetProperties();
        props.Should().NotContain(p => p.Name == "CreatedOnUtc",
            "CreatedOnUtc was removed — InvoiceLine doesn't need auditing");
    }

    [Fact]
    public void InvoiceLine_DoesNotHave_ModifiedOnUtc_Property()
    {
        var props = typeof(InvoiceLine).GetProperties();
        props.Should().NotContain(p => p.Name == "ModifiedOnUtc",
            "ModifiedOnUtc was removed — InvoiceLine doesn't need auditing");
    }

    // ─────────────────────────────────────────────────────────────
    // Sanity: Create still works after the fix
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void InvoiceLine_Create_StillWorksAfterFix()
    {
        var result = InvoiceLine.Create(
            Guid.NewGuid(), Guid.NewGuid(), 5, "PCS", 100m, "EGP");

        result.IsSuccess.Should().BeTrue();
        result.Value.SubTotal.Should().Be(500m);
    }
}
