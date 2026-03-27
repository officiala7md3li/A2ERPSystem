using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using FluentAssertions;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class DebitNoteEntityTests
{
    private static DebitNote CreateDraftNote()
    {
        var result = DebitNote.Create(
            customerId: Guid.NewGuid(),
            companyId: Guid.NewGuid(),
            currencyId: Guid.NewGuid(),
            originalInvoiceId: Guid.NewGuid(),
            noteDate: DateTime.UtcNow,
            reason: "Price adjustment");
        return result.Value;
    }

    [Fact]
    public void DebitNote_Create_WithValidData_Succeeds()
    {
        var result = DebitNote.Create(
            customerId: Guid.NewGuid(), companyId: Guid.NewGuid(), currencyId: Guid.NewGuid(),
            originalInvoiceId: Guid.NewGuid(), noteDate: DateTime.UtcNow, reason: "Shortage adjustment");


        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(InvoiceStatus.Draft);
    }

    [Fact]
    public void DebitNote_Create_WithEmptyCustomerId_Fails()
    {
        var result = DebitNote.Create(
            customerId: Guid.Empty, companyId: Guid.NewGuid(), currencyId: Guid.NewGuid(),
            originalInvoiceId: Guid.NewGuid(), noteDate: DateTime.UtcNow, reason: "reason");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void DebitNote_Submit_WithLines_ChangeStatusToPending()
    {
        var note = CreateDraftNote();
        var line = InvoiceLine.Create(note.Id, Guid.NewGuid(), 1, "PCS", 200m, "EGP").Value;
        note.AddLine(line);

        var result = note.Submit();

        result.IsSuccess.Should().BeTrue();
        note.Status.Should().Be(InvoiceStatus.Pending);
    }

    [Fact]
    public void DebitNote_Post_WhenPending_AssignsSequenceAndPostedStatus()
    {
        var note = CreateDraftNote();
        note.AddLine(InvoiceLine.Create(note.Id, Guid.NewGuid(), 1, "PCS", 200m, "EGP").Value);
        note.Submit();

        var result = note.Post("DBN-20260324-000001", "{}");

        result.IsSuccess.Should().BeTrue();
        note.Status.Should().Be(InvoiceStatus.Posted);
        note.SequenceNumber.Should().Be("DBN-20260324-000001");
    }

    [Fact]
    public void DebitNote_Cancel_WhenPending_Succeeds()
    {
        var note = CreateDraftNote();
        note.AddLine(InvoiceLine.Create(note.Id, Guid.NewGuid(), 1, "PCS", 200m, "EGP").Value);
        note.Submit(); // → Pending

        var result = note.Cancel();

        result.IsSuccess.Should().BeTrue();
        note.Status.Should().Be(InvoiceStatus.Cancelled);
    }
}
