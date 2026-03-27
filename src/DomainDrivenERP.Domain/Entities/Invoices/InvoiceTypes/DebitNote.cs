using DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Invoices;

/// <summary>
/// إشعار مدين — يُنشأ لإضافة مبالغ إضافية على فاتورة عميل
/// </summary>
public sealed class DebitNote : AggregateRoot, IAuditableEntity
{
    private readonly List<InvoiceLine> _lines = new();

    public DebitNote() { }

    private DebitNote(
        Guid id,
        Guid customerId,
        Guid companyId,
        Guid currencyId,
        Guid originalInvoiceId,
        DateTime noteDate,
        string reason) : base(id)
    {
        CustomerId = customerId;
        CompanyId = companyId;
        CurrencyId = currencyId;
        OriginalInvoiceId = originalInvoiceId;
        NoteDate = noteDate;
        Reason = reason;
        Status = InvoiceStatus.Draft;
    }

    public Guid CustomerId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid CurrencyId { get; private set; }
    public Guid OriginalInvoiceId { get; private set; }
    public string? SequenceNumber { get; private set; }
    public DateTime NoteDate { get; private set; }
    public string Reason { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal TotalTax { get; private set; }
    public decimal GrandTotal { get; private set; }
    public string? PipelineSnapshot { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public IReadOnlyCollection<InvoiceLine> Lines => _lines;

    public static Result<DebitNote> Create(
        Guid customerId,
        Guid companyId,
        Guid currencyId,
        Guid originalInvoiceId,
        DateTime noteDate,
        string reason)
    {
        if (customerId == Guid.Empty)
            return Result.Failure<DebitNote>(new Error("DebitNote.InvalidCustomer", "Customer ID is required."));
        if (originalInvoiceId == Guid.Empty)
            return Result.Failure<DebitNote>(new Error("DebitNote.InvalidOriginalInvoice", "Original Invoice ID is required."));
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure<DebitNote>(new Error("DebitNote.InvalidReason", "Reason is required."));

        var note = new DebitNote(Guid.NewGuid(), customerId, companyId, currencyId, originalInvoiceId, noteDate, reason);
        note.RaiseDomainEvent(new DebitNoteCreatedDomainEvent(note.Id, customerId, originalInvoiceId));
        return Result.Success(note);
    }

    public Result AddLine(InvoiceLine line)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("DebitNote.NotDraft", "Cannot modify a posted debit note."));
        _lines.Add(line);
        RecalculateTotals();
        return Result.Success();
    }

    public Result Post(string sequenceNumber, string pipelineSnapshot)
    {
        if (Status != InvoiceStatus.Pending)
            return Result.Failure(new Error("DebitNote.NotPending", "Only pending notes can be posted."));
        SequenceNumber = sequenceNumber;
        PipelineSnapshot = pipelineSnapshot;
        Status = InvoiceStatus.Posted;
        RaiseDomainEvent(new DebitNotePostedDomainEvent(Id, CustomerId, OriginalInvoiceId, GrandTotal));
        return Result.Success();
    }

    public Result Submit()
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("DebitNote.NotDraft", "Only draft notes can be submitted."));
        Status = InvoiceStatus.Pending;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is InvoiceStatus.Cancelled or InvoiceStatus.Posted)
            return Result.Failure(new Error("DebitNote.CannotCancel", "Note cannot be cancelled."));
        Status = InvoiceStatus.Cancelled;
        return Result.Success();
    }

    private void RecalculateTotals()
    {
        TotalAmount = _lines.Sum(l => l.SubTotal - l.TotalDiscountAmount);
        TotalTax = _lines.Sum(l => l.TotalTaxAmount);
        GrandTotal = TotalAmount + TotalTax;
    }
}
