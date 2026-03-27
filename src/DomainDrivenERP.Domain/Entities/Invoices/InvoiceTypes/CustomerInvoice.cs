using DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Guards;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Invoices;

public sealed class CustomerInvoice : AggregateRoot, IAuditableEntity
{
    private readonly List<InvoiceLine> _lines = new();
    private readonly List<InvoiceLevelDiscount> _invoiceDiscounts = new();

    // Required for Dapper
    public CustomerInvoice() { }

    private CustomerInvoice(
        Guid id,
        Guid customerId,
        Guid companyId,
        Guid currencyId,
        DateTime invoiceDate,
        TaxOrderSetting taxOrderSetting,
        StackingMode stackingMode) : base(id)
    {
        CustomerId = customerId;
        CompanyId = companyId;
        CurrencyId = currencyId;
        InvoiceDate = invoiceDate;
        TaxOrderSetting = taxOrderSetting;
        StackingMode = stackingMode;
        Status = InvoiceStatus.Draft;
    }

    // ── Identity ──────────────────────────────────────────
    public Guid CustomerId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid CurrencyId { get; private set; }
    public string? SequenceNumber { get; private set; }   // يتحدد من Sequence Engine

    // ── Dates ─────────────────────────────────────────────
    public DateTime InvoiceDate { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // ── Status ────────────────────────────────────────────
    public InvoiceStatus Status { get; private set; }

    // ── Pipeline Settings (محفوظة على الفاتورة) ──────────
    public TaxOrderSetting TaxOrderSetting { get; private set; }
    public StackingMode StackingMode { get; private set; }

    // ── Totals ────────────────────────────────────────────
    public decimal SubTotal { get; private set; }
    public decimal TotalLineDiscount { get; private set; }
    public decimal TotalTax { get; private set; }
    public decimal TotalInvoiceDiscount { get; private set; }
    public decimal TotalHiddenDiscount { get; private set; }
    public decimal GrandTotal { get; private set; }

    // ── Invoice Level Hidden Discount ─────────────────────
    public decimal InvoiceHiddenDiscountAmount { get; private set; }
    public HiddenDiscountType InvoiceHiddenDiscountType { get; private set; }

    // ── Snapshot (محفوظ عند الـ Posting) ─────────────────
    public string? PipelineSnapshot { get; private set; } // JSON

    // ── Notes ─────────────────────────────────────────────
    public string? Notes { get; private set; }

    // ── Collections ───────────────────────────────────────
    public IReadOnlyCollection<InvoiceLine> Lines => _lines;
    public IReadOnlyCollection<InvoiceLevelDiscount> InvoiceDiscounts => _invoiceDiscounts;

    // ── Factory ───────────────────────────────────────────
    public static Result<CustomerInvoice> Create(
        Guid customerId,
        Guid companyId,
        Guid currencyId,
        DateTime invoiceDate,
        TaxOrderSetting taxOrderSetting = TaxOrderSetting.AfterDiscount,
        StackingMode stackingMode = StackingMode.NoStacking,
        string? notes = null)
    {
        if (customerId == Guid.Empty)
            return Result.Failure<CustomerInvoice>(new Error("CustomerInvoice.InvalidCustomer", "Customer ID is required."));

        if (companyId == Guid.Empty)
            return Result.Failure<CustomerInvoice>(new Error("CustomerInvoice.InvalidCompany", "Company ID is required."));

        if (currencyId == Guid.Empty)
            return Result.Failure<CustomerInvoice>(new Error("CustomerInvoice.InvalidCurrency", "Currency ID is required."));

        var invoice = new CustomerInvoice(
            Guid.NewGuid(),
            customerId,
            companyId,
            currencyId,
            invoiceDate,
            taxOrderSetting,
            stackingMode)
        {
            Notes = notes
        };

        invoice.RaiseDomainEvent(new CustomerInvoiceCreatedDomainEvent(invoice.Id, customerId, companyId));
        return Result.Success(invoice);
    }

    // ── Line Management ───────────────────────────────────
    public Result AddLine(InvoiceLine line)
    {
        Guard.Against.Null(line, nameof(line));

        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("CustomerInvoice.NotDraft", "Cannot modify a non-draft invoice."));

        if (line.InvoiceId != Id)
            return Result.Failure(new Error("CustomerInvoice.LineMismatch", "Line does not belong to this invoice."));

        _lines.Add(line);
        RecalculateTotals();
        return Result.Success();
    }

    public Result RemoveLine(Guid lineId)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("CustomerInvoice.NotDraft", "Cannot modify a non-draft invoice."));

        InvoiceLine? line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result.Failure(new Error("CustomerInvoice.LineNotFound", "Line not found."));

        _lines.Remove(line);
        RecalculateTotals();
        return Result.Success();
    }

    // ── Invoice Level Discount ────────────────────────────
    public Result AddInvoiceDiscount(InvoiceLevelDiscount discount)
    {
        Guard.Against.Null(discount, nameof(discount));

        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("CustomerInvoice.NotDraft", "Cannot modify a non-draft invoice."));

        _invoiceDiscounts.Add(discount);
        RecalculateTotals();
        return Result.Success();
    }

    // ── Hidden Discount على مستوى الـ Invoice ─────────────
    public Result SetInvoiceHiddenDiscount(decimal value, HiddenDiscountType type)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("CustomerInvoice.NotDraft", "Cannot modify a non-draft invoice."));

        if (value < 0)
            return Result.Failure(new Error("CustomerInvoice.InvalidHiddenDiscount", "Hidden discount cannot be negative."));

        InvoiceHiddenDiscountType = type;
        InvoiceHiddenDiscountAmount = type switch
        {
            HiddenDiscountType.Percentage => (SubTotal - TotalLineDiscount + TotalTax - TotalInvoiceDiscount) * (value / 100),
            HiddenDiscountType.FixedAmount => value,
            _ => 0
        };

        RecalculateTotals();
        return Result.Success();
    }

    // ── Status Transitions ────────────────────────────────
    public Result Submit()
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("CustomerInvoice.NotDraft", "Only draft invoices can be submitted."));

        if (!_lines.Any())
            return Result.Failure(new Error("CustomerInvoice.NoLines", "Invoice must have at least one line."));

        Status = InvoiceStatus.Pending;
        return Result.Success();
    }

    public Result Post(string sequenceNumber, string pipelineSnapshot)
    {
        if (Status != InvoiceStatus.Pending)
            return Result.Failure(new Error("CustomerInvoice.NotPending", "Only pending invoices can be posted."));

        SequenceNumber = sequenceNumber;
        PipelineSnapshot = pipelineSnapshot;
        PostedAt = DateTime.UtcNow;
        Status = InvoiceStatus.Posted;

        RaiseDomainEvent(new CustomerInvoicePostedDomainEvent(Id, CustomerId, CompanyId, GrandTotal));
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == InvoiceStatus.Cancelled)
            return Result.Failure(new Error("CustomerInvoice.AlreadyCancelled", "Invoice is already cancelled."));

        if (Status == InvoiceStatus.Paid)
            return Result.Failure(new Error("CustomerInvoice.AlreadyPaid", "Cannot cancel a paid invoice."));

        Status = InvoiceStatus.Cancelled;
        RaiseDomainEvent(new CustomerInvoiceCancelledDomainEvent(Id, CustomerId, CompanyId));
        return Result.Success();
    }

    // ── Sequence ──────────────────────────────────────────
    public Result AssignSequenceNumber(string sequenceNumber)
    {
        Guard.Against.NullOrWhiteSpace(sequenceNumber, nameof(sequenceNumber));

        if (!string.IsNullOrWhiteSpace(SequenceNumber))
            return Result.Failure(new Error("CustomerInvoice.SequenceAlreadyAssigned", "Sequence number already assigned."));

        SequenceNumber = sequenceNumber;
        return Result.Success();
    }

    // ── Recalculate ───────────────────────────────────────
    private void RecalculateTotals()
    {
        SubTotal = _lines.Sum(l => l.SubTotal);
        TotalLineDiscount = _lines.Sum(l => l.TotalDiscountAmount);
        TotalTax = _lines.Sum(l => l.TotalTaxAmount);
        TotalInvoiceDiscount = _invoiceDiscounts.Sum(d => d.DiscountAmount);
        TotalHiddenDiscount = _lines.Sum(l => l.HiddenDiscountAmount) + InvoiceHiddenDiscountAmount;

        GrandTotal = SubTotal
            - TotalLineDiscount
            + TotalTax
            - TotalInvoiceDiscount
            - TotalHiddenDiscount;
    }
}
