using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class CustomerInvoiceEntityTests
{
    private static CustomerInvoice CreateDraftInvoice()
    {
        var result = CustomerInvoice.Create(
            customerId: Guid.NewGuid(),
            companyId: Guid.NewGuid(),
            currencyId: Guid.NewGuid(),
            invoiceDate: DateTime.UtcNow);
        return result.Value;
    }

    // ─────────────────────────────────────────────────────────────
    // 1. Create succeeds with valid data
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CustomerInvoice_Create_WithValidData_Succeeds()
    {
        var result = CustomerInvoice.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(InvoiceStatus.Draft);
        result.Value.GrandTotal.Should().Be(0);
    }

    [Fact]
    public void CustomerInvoice_Create_WithEmptyCustomerId_Fails()
    {
        var result = CustomerInvoice.Create(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CustomerInvoice.InvalidCustomer");
    }

    // ─────────────────────────────────────────────────────────────
    // 2. Submit — Draft → Pending
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CustomerInvoice_Submit_WithNoLines_Fails()
    {
        var invoice = CreateDraftInvoice();

        var result = invoice.Submit();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CustomerInvoice.NoLines");
    }

    [Fact]
    public void CustomerInvoice_Submit_WithLines_ChangeStatusToPending()
    {
        var invoice = CreateDraftInvoice();
        var line = InvoiceLine.Create(
            invoice.Id, Guid.NewGuid(), 2, "PCS", 500m, "EGP").Value;
        invoice.AddLine(line);

        var result = invoice.Submit();

        result.IsSuccess.Should().BeTrue();
        invoice.Status.Should().Be(InvoiceStatus.Pending);
    }

    // ─────────────────────────────────────────────────────────────
    // 3. Post — Pending → Posted
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CustomerInvoice_Post_WhenPending_AssignsSequenceAndChangesStatus()
    {
        var invoice = CreateDraftInvoice();
        invoice.AddLine(InvoiceLine.Create(invoice.Id, Guid.NewGuid(), 1, "PCS", 1000m, "EGP").Value);
        invoice.Submit();

        var result = invoice.Post("INV-20260324-000001", "{\"tax\":\"14%\"}");

        result.IsSuccess.Should().BeTrue();
        invoice.Status.Should().Be(InvoiceStatus.Posted);
        invoice.SequenceNumber.Should().Be("INV-20260324-000001");
        invoice.PostedAt.Should().NotBeNull();
    }

    [Fact]
    public void CustomerInvoice_Post_WhenNotPending_Fails()
    {
        var invoice = CreateDraftInvoice();

        var result = invoice.Post("INV-001", "{}");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CustomerInvoice.NotPending");
    }

    // ─────────────────────────────────────────────────────────────
    // 4. Cancel — Posted → Cancelled
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CustomerInvoice_Cancel_WhenPosted_Succeeds()
    {
        var invoice = CreateDraftInvoice();
        invoice.AddLine(InvoiceLine.Create(invoice.Id, Guid.NewGuid(), 1, "PCS", 1000m, "EGP").Value);
        invoice.Submit();
        invoice.Post("INV-001", "{}");

        var result = invoice.Cancel();

        result.IsSuccess.Should().BeTrue();
        invoice.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void CustomerInvoice_Cancel_WhenAlreadyCancelled_Fails()
    {
        var invoice = CreateDraftInvoice();
        invoice.AddLine(InvoiceLine.Create(invoice.Id, Guid.NewGuid(), 1, "PCS", 1000m, "EGP").Value);
        invoice.Submit();
        invoice.Post("INV-001", "{}");
        invoice.Cancel();

        var result = invoice.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CustomerInvoice.AlreadyCancelled");
    }

    // ─────────────────────────────────────────────────────────────
    // 5. Totals recalculate correctly
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CustomerInvoice_RecalculateTotals_ComputesSubTotalAndGrandTotal()
    {
        var invoice = CreateDraftInvoice();
        invoice.AddLine(InvoiceLine.Create(invoice.Id, Guid.NewGuid(), 2, "PCS", 500m, "EGP").Value);
        invoice.AddLine(InvoiceLine.Create(invoice.Id, Guid.NewGuid(), 3, "PCS", 100m, "EGP").Value);

        // SubTotal = (2 * 500) + (3 * 100) = 1000 + 300 = 1300
        invoice.SubTotal.Should().Be(1300m);
        invoice.GrandTotal.Should().Be(1300m, "no tax or discount applied yet");
    }

    // ─────────────────────────────────────────────────────────────
    // 6. AddLine — cannot modify non-draft invoice
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void CustomerInvoice_AddLine_WhenNotDraft_Fails()
    {
        var invoice = CreateDraftInvoice();
        invoice.AddLine(InvoiceLine.Create(invoice.Id, Guid.NewGuid(), 1, "PCS", 100m, "EGP").Value);
        invoice.Submit();

        var newLine = InvoiceLine.Create(invoice.Id, Guid.NewGuid(), 1, "PCS", 200m, "EGP").Value;
        var result = invoice.AddLine(newLine);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CustomerInvoice.NotDraft");
    }
}
