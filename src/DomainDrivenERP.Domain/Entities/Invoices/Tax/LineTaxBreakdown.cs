using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices;

public sealed class LineTaxBreakdown : BaseEntity
{
    private LineTaxBreakdown() { }

    internal LineTaxBreakdown(
        Guid id,
        Guid invoiceLineId,
        Guid taxDefinitionId,
        string taxCode,
        string taxName,
        decimal rate,
        decimal taxAmount,
        bool isWithholding) : base(id)
    {
        InvoiceLineId = invoiceLineId;
        TaxDefinitionId = taxDefinitionId;
        TaxCode = taxCode;
        TaxName = taxName;
        Rate = rate;
        TaxAmount = taxAmount;
        IsWithholding = isWithholding;
    }

    public static LineTaxBreakdown Create(
        Guid invoiceLineId,
        Guid taxDefinitionId,
        string taxCode,
        string taxName,
        decimal rate,
        decimal taxAmount,
        bool isWithholding)
    {
        return new LineTaxBreakdown(
            Guid.NewGuid(),
            invoiceLineId,
            taxDefinitionId,
            taxCode,
            taxName,
            rate,
            taxAmount,
            isWithholding);
    }

    public Guid InvoiceLineId { get; private set; }
    public Guid TaxDefinitionId { get; private set; }
    public string TaxCode { get; private set; }       // "VAT", "ST01", "Tbl01"
    public string TaxName { get; private set; }
    public decimal Rate { get; private set; }          // النسبة أو الصفر للثابت
    public decimal TaxAmount { get; private set; }     // المبلغ المحسوب
    public bool IsWithholding { get; private set; }    // W codes — بيطرح مش بيضيف
}

