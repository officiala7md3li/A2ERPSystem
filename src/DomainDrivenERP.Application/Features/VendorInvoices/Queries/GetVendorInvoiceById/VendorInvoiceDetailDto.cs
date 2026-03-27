using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Features.VendorInvoices.Queries.GetVendorInvoiceById;

public sealed record VendorInvoiceDetailDto(
    Guid Id,
    Guid VendorId,
    Guid CompanyId,
    string? SequenceNumber,
    string VendorInvoiceNumber,
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
    IReadOnlyList<VendorInvoiceLineDto> Lines);

public sealed record VendorInvoiceLineDto(
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
    IReadOnlyList<VendorLineTaxBreakdownDto> TaxBreakdowns,
    IReadOnlyList<VendorLineDiscountBreakdownDto> DiscountBreakdowns);

public sealed record VendorLineTaxBreakdownDto(
    string TaxCode,
    string TaxName,
    decimal Rate,
    decimal TaxAmount,
    bool IsWithholding);

public sealed record VendorLineDiscountBreakdownDto(
    string Source,
    string Type,
    decimal DiscountValue,
    decimal DiscountAmount);
