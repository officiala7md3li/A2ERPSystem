using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices;

public sealed class InvoiceLevelDiscount : BaseEntity
{
    private InvoiceLevelDiscount() { }

    internal InvoiceLevelDiscount(
        Guid id,
        Guid invoiceId,
        DiscountSource source,
        DiscountType type,
        decimal discountValue,
        decimal discountAmount,
        Guid? referenceId = null) : base(id)
    {
        InvoiceId = invoiceId;
        Source = source;
        Type = type;
        DiscountValue = discountValue;
        DiscountAmount = discountAmount;
        ReferenceId = referenceId;
    }

    public Guid InvoiceId { get; private set; }
    public DiscountSource Source { get; private set; }
    public DiscountType Type { get; private set; }
    public decimal DiscountValue { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public Guid? ReferenceId { get; private set; }
}
