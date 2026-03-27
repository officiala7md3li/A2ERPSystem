using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Guards;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Domain.ValueObjects;

namespace DomainDrivenERP.Domain.Entities.Invoices;

public sealed class InvoiceLine : AggregateRoot, IAuditableEntity
{
    private readonly List<LineTaxBreakdown> _taxBreakdowns = new();
    private readonly List<LineDiscountBreakdown> _discountBreakdowns = new();

    // Required for Dapper
    public InvoiceLine() { }

    private InvoiceLine(
        Guid id,
        Guid invoiceId,
        Guid itemId,
        Quantity quantity,
        Money unitPrice,
        Guid? taxGroupId,
        bool isTaxOverridden,
        Guid? discountGroupId,
        bool isDiscountOverridden,
        int sortOrder) : base(id)
    {
        InvoiceId = invoiceId;
        ItemId = itemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxGroupId = taxGroupId;
        IsTaxOverridden = isTaxOverridden;
        DiscountGroupId = discountGroupId;
        IsDiscountOverridden = isDiscountOverridden;
        SortOrder = sortOrder;
    }

    public Guid InvoiceId { get; private set; }
    public Guid ItemId { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }

    // ── Discount ──────────────────────────────────────────
    public Guid? DiscountGroupId { get; private set; }
    public bool IsDiscountOverridden { get; private set; }
    public decimal TotalDiscountAmount { get; private set; }

    // ── Tax ───────────────────────────────────────────────
    public Guid? TaxGroupId { get; private set; }
    public bool IsTaxOverridden { get; private set; }
    public decimal TotalTaxAmount { get; private set; }

    // ── Hidden Discount (بعد الضريبة) ────────────────────
    public decimal HiddenDiscountAmount { get; private set; }
    public HiddenDiscountType HiddenDiscountType { get; private set; }

    // ── Totals ────────────────────────────────────────────
    public decimal SubTotal => Quantity.Value * UnitPrice.Amount;
    public decimal NetAfterDiscount => SubTotal - TotalDiscountAmount;
    public decimal NetAfterTax => NetAfterDiscount + TotalTaxAmount;
    public decimal FinalLineTotal => NetAfterTax - HiddenDiscountAmount;

    // ── Snapshots ─────────────────────────────────────────
    public string? TaxGroupSnapshot { get; private set; }      // JSON
    public string? DiscountGroupSnapshot { get; private set; } // JSON

    public int SortOrder { get; private set; }

    // ── Collections ───────────────────────────────────────
    public IReadOnlyCollection<LineTaxBreakdown> TaxBreakdowns => _taxBreakdowns;
    public IReadOnlyCollection<LineDiscountBreakdown> DiscountBreakdowns => _discountBreakdowns;

    public DateTime CreatedOnUtc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? ModifiedOnUtc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    // ── Factory ───────────────────────────────────────────
    public static Result<InvoiceLine> Create(
        Guid invoiceId,
        Guid itemId,
        decimal quantityValue,
        string quantityUnit,
        decimal unitPriceAmount,
        string currency,
        Guid? taxGroupId = null,
        Guid? discountGroupId = null,
        int sortOrder = 0)
    {
        if (invoiceId == Guid.Empty)
            return Result.Failure<InvoiceLine>(new Error("InvoiceLine.InvalidInvoiceId", "Invoice ID is required."));

        if (itemId == Guid.Empty)
            return Result.Failure<InvoiceLine>(new Error("InvoiceLine.InvalidItemId", "Item ID is required."));

        Result<Quantity> quantityResult = Quantity.Create(quantityValue, quantityUnit);
        if (quantityResult.IsFailure)
            return Result.Failure<InvoiceLine>(quantityResult.Error);

        Result<Money> priceResult = Money.Create(unitPriceAmount, currency);
        if (priceResult.IsFailure)
            return Result.Failure<InvoiceLine>(priceResult.Error);

        var line = new InvoiceLine(
            Guid.NewGuid(),
            invoiceId,
            itemId,
            quantityResult.Value,
            priceResult.Value,
            taxGroupId,
            isTaxOverridden: taxGroupId.HasValue,
            discountGroupId,
            isDiscountOverridden: discountGroupId.HasValue,
            sortOrder);

        return Result.Success(line);
    }

    // ── Domain Methods ────────────────────────────────────
    public void SetTaxBreakdown(IEnumerable<LineTaxBreakdown> breakdowns)
    {
        _taxBreakdowns.Clear();
        _taxBreakdowns.AddRange(breakdowns);
        TotalTaxAmount = _taxBreakdowns.Sum(t => t.IsWithholding ? -t.TaxAmount : t.TaxAmount);
    }

    public void SetDiscountBreakdown(IEnumerable<LineDiscountBreakdown> breakdowns)
    {
        _discountBreakdowns.Clear();
        _discountBreakdowns.AddRange(breakdowns);
        TotalDiscountAmount = _discountBreakdowns.Sum(d => d.DiscountAmount);
    }

    public Result SetHiddenDiscount(decimal value, HiddenDiscountType type)
    {
        if (value < 0)
            return Result.Failure(new Error("InvoiceLine.InvalidHiddenDiscount", "Hidden discount cannot be negative."));

        HiddenDiscountType = type;
        HiddenDiscountAmount = type switch
        {
            HiddenDiscountType.Percentage => NetAfterTax * (value / 100),
            HiddenDiscountType.FixedAmount => value,
            _ => 0
        };

        return Result.Success();
    }

    public void SetSnapshots(string? taxSnapshot, string? discountSnapshot)
    {
        TaxGroupSnapshot = taxSnapshot;
        DiscountGroupSnapshot = discountSnapshot;
    }

    public void OverrideTaxGroup(Guid taxGroupId)
    {
        TaxGroupId = taxGroupId;
        IsTaxOverridden = true;
    }

    public void OverrideDiscountGroup(Guid discountGroupId)
    {
        DiscountGroupId = discountGroupId;
        IsDiscountOverridden = true;
    }
}
