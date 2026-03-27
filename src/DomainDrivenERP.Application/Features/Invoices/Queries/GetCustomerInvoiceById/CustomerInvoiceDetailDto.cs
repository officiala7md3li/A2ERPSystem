using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoiceById;

public sealed record CustomerInvoiceDetailDto(
    Guid Id,
    Guid CustomerId,
    Guid CompanyId,
    string? SequenceNumber,
    DateTime InvoiceDate,
    string Status,
    decimal SubTotal,
    decimal TotalLineDiscount,
    decimal TotalTax,
    decimal TotalInvoiceDiscount,
    decimal TotalHiddenDiscount,
    decimal GrandTotal,
    string TaxOrderSetting,
    string StackingMode,
    IReadOnlyList<InvoiceLineDto> Lines);

public sealed record InvoiceLineDto(
    Guid Id,
    Guid ItemId,
    decimal Quantity,
    string QuantityUnit,
    decimal UnitPrice,
    string Currency,
    decimal SubTotal,
    decimal TotalDiscountAmount,
    decimal TotalTaxAmount,
    decimal HiddenDiscountAmount,
    decimal FinalLineTotal,
    IReadOnlyList<LineTaxBreakdownDto> TaxBreakdowns,
    IReadOnlyList<LineDiscountBreakdownDto> DiscountBreakdowns);

public sealed record LineTaxBreakdownDto(
    string TaxCode,
    string TaxName,
    decimal Rate,
    decimal TaxAmount,
    bool IsWithholding);

public sealed record LineDiscountBreakdownDto(
    string Source,
    string Type,
    decimal DiscountValue,
    decimal DiscountAmount);
