using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class CreditNoteEntityTests
{
    private static CreditNote CreateDraftNote()
    {
        var result = CreditNote.Create(
            customerId: Guid.NewGuid(),
            companyId: Guid.NewGuid(),
            currencyId: Guid.NewGuid(),
            originalInvoiceId: Guid.NewGuid(),
            noteDate: DateTime.UtcNow,
            reason: "Return of goods");
        return result.Value;
    }

    [Fact]
    public void CreditNote_Create_WithValidData_Succeeds()
    {
        var result = CreditNote.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), DateTime.UtcNow, "Damaged goods");

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(InvoiceStatus.Draft);
    }

    [Fact]
    public void CreditNote_Create_WithEmptyCustomerId_Fails()
    {
        var result = CreditNote.Create(
            Guid.Empty, Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), DateTime.UtcNow, "reason");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CreditNote_Submit_WithLines_ChangeStatusToPending()
    {
        var note = CreateDraftNote();
        var line = InvoiceLine.Create(note.Id, Guid.NewGuid(), 1, "PCS", 100m, "EGP").Value;
        note.AddLine(line);

        var result = note.Submit();

        result.IsSuccess.Should().BeTrue();
        note.Status.Should().Be(InvoiceStatus.Pending);
    }

    [Fact]
    public void CreditNote_Post_WhenPending_AssignsSequenceAndPostedStatus()
    {
        var note = CreateDraftNote();
        note.AddLine(InvoiceLine.Create(note.Id, Guid.NewGuid(), 1, "PCS", 100m, "EGP").Value);
        note.Submit();

        var result = note.Post("CRN-20260324-000001", "{}");

        result.IsSuccess.Should().BeTrue();
        note.Status.Should().Be(InvoiceStatus.Posted);
        note.SequenceNumber.Should().Be("CRN-20260324-000001");
    }

    [Fact]
    public void CreditNote_Cancel_WhenPending_Succeeds()
    {
        var note = CreateDraftNote();
        note.AddLine(InvoiceLine.Create(note.Id, Guid.NewGuid(), 1, "PCS", 100m, "EGP").Value);
        note.Submit(); // → Pending

        var result = note.Cancel();

        result.IsSuccess.Should().BeTrue();
        note.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void CreditNote_Cancel_WhenPosted_Fails()
    {
        var note = CreateDraftNote();
        note.AddLine(InvoiceLine.Create(note.Id, Guid.NewGuid(), 1, "PCS", 100m, "EGP").Value);
        note.Submit();
        note.Post("CRN-001", "{}");

        var result = note.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CreditNote.CannotCancel");
    }

}
