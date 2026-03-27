using DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Guards;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.Invoices;

public sealed class VendorInvoice : AggregateRoot, IAuditableEntity
{
    private readonly List<InvoiceLine> _lines = new();
    private readonly List<InvoiceLevelDiscount> _invoiceDiscounts = new();

    public VendorInvoice() { }

    private VendorInvoice(
        Guid id,
        Guid vendorId,
        Guid companyId,
        Guid currencyId,
        DateTime invoiceDate,
        string? vendorInvoiceNumber,
        TaxOrderSetting taxOrderSetting,
        StackingMode stackingMode) : base(id)
    {
        VendorId = vendorId;
        CompanyId = companyId;
        CurrencyId = currencyId;
        InvoiceDate = invoiceDate;
        VendorInvoiceNumber = vendorInvoiceNumber;
        TaxOrderSetting = taxOrderSetting;
        StackingMode = stackingMode;
        Status = InvoiceStatus.Draft;
    }

    public Guid VendorId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid CurrencyId { get; private set; }
    public string? SequenceNumber { get; private set; }
    public string? VendorInvoiceNumber { get; private set; } // رقم فاتورة المورد الأصلية
    public DateTime InvoiceDate { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public InvoiceStatus Status { get; private set; }
    public TaxOrderSetting TaxOrderSetting { get; private set; }
    public StackingMode StackingMode { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TotalLineDiscount { get; private set; }
    public decimal TotalTax { get; private set; }
    public decimal TotalInvoiceDiscount { get; private set; }
    public decimal TotalHiddenDiscount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public decimal InvoiceHiddenDiscountAmount { get; private set; }
    public HiddenDiscountType InvoiceHiddenDiscountType { get; private set; }
    public string? PipelineSnapshot { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<InvoiceLine> Lines => _lines;
    public IReadOnlyCollection<InvoiceLevelDiscount> InvoiceDiscounts => _invoiceDiscounts;

    public static Result<VendorInvoice> Create(
        Guid vendorId,
        Guid companyId,
        Guid currencyId,
        DateTime invoiceDate,
        string? vendorInvoiceNumber = null,
        TaxOrderSetting taxOrderSetting = TaxOrderSetting.AfterDiscount,
        StackingMode stackingMode = StackingMode.NoStacking,
        string? notes = null)
    {
        if (vendorId == Guid.Empty)
            return Result.Failure<VendorInvoice>(new Error("VendorInvoice.InvalidVendor", "Vendor ID is required."));

        if (companyId == Guid.Empty)
            return Result.Failure<VendorInvoice>(new Error("VendorInvoice.InvalidCompany", "Company ID is required."));

        if (currencyId == Guid.Empty)
            return Result.Failure<VendorInvoice>(new Error("VendorInvoice.InvalidCurrency", "Currency ID is required."));

        var invoice = new VendorInvoice(
            Guid.NewGuid(), vendorId, companyId, currencyId,
            invoiceDate, vendorInvoiceNumber, taxOrderSetting, stackingMode)
        { Notes = notes };

        invoice.RaiseDomainEvent(new VendorInvoiceCreatedDomainEvent(invoice.Id, vendorId, companyId));
        return Result.Success(invoice);
    }

    public Result AddLine(InvoiceLine line)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("VendorInvoice.NotDraft", "Cannot modify a non-draft invoice."));
        if (line.InvoiceId != Id)
            return Result.Failure(new Error("VendorInvoice.LineMismatch", "Line does not belong to this invoice."));
        _lines.Add(line);
        RecalculateTotals();
        return Result.Success();
    }

    public Result Post(string sequenceNumber, string pipelineSnapshot)
    {
        if (Status != InvoiceStatus.Pending)
            return Result.Failure(new Error("VendorInvoice.NotPending", "Only pending invoices can be posted."));
        SequenceNumber = sequenceNumber;
        PipelineSnapshot = pipelineSnapshot;
        PostedAt = DateTime.UtcNow;
        Status = InvoiceStatus.Posted;
        RaiseDomainEvent(new VendorInvoicePostedDomainEvent(Id, VendorId, CompanyId, GrandTotal));
        return Result.Success();
    }

    public Result Submit()
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("VendorInvoice.NotDraft", "Only draft invoices can be submitted."));
        if (!_lines.Any())
            return Result.Failure(new Error("VendorInvoice.NoLines", "Invoice must have at least one line."));
        Status = InvoiceStatus.Pending;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is InvoiceStatus.Cancelled or InvoiceStatus.Paid)
            return Result.Failure(new Error("VendorInvoice.CannotCancel", "Invoice cannot be cancelled."));
        Status = InvoiceStatus.Cancelled;
        RaiseDomainEvent(new VendorInvoiceCancelledDomainEvent(Id, VendorId, CompanyId));
        return Result.Success();
    }

    private void RecalculateTotals()
    {
        SubTotal = _lines.Sum(l => l.SubTotal);
        TotalLineDiscount = _lines.Sum(l => l.TotalDiscountAmount);
        TotalTax = _lines.Sum(l => l.TotalTaxAmount);
        TotalInvoiceDiscount = _invoiceDiscounts.Sum(d => d.DiscountAmount);
        TotalHiddenDiscount = _lines.Sum(l => l.HiddenDiscountAmount) + InvoiceHiddenDiscountAmount;
        GrandTotal = SubTotal - TotalLineDiscount + TotalTax - TotalInvoiceDiscount - TotalHiddenDiscount;
    }
}
