using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices;

public sealed class LineDiscountBreakdown : BaseEntity
{
    private LineDiscountBreakdown() { }

    internal LineDiscountBreakdown(
        Guid id,
        Guid invoiceLineId,
        DiscountSource source,
        DiscountType type,
        decimal discountValue,
        decimal discountAmount,
        Guid? referenceId = null) : base(id)
    {
        InvoiceLineId = invoiceLineId;
        Source = source;
        Type = type;
        DiscountValue = discountValue;
        DiscountAmount = discountAmount;
        ReferenceId = referenceId;
    }

    public static LineDiscountBreakdown Create(
        Guid invoiceLineId,
        DiscountSource source,
        DiscountType type,
        decimal discountValue,
        decimal discountAmount,
        Guid? referenceId = null)
    {
        return new LineDiscountBreakdown(
            Guid.NewGuid(),
            invoiceLineId,
            source,
            type,
            discountValue,
            discountAmount,
            referenceId);
    }

    public Guid InvoiceLineId { get; private set; }
    public DiscountSource Source { get; private set; }
    public DiscountType Type { get; private set; }
    public decimal DiscountValue { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public Guid? ReferenceId { get; private set; }
}

