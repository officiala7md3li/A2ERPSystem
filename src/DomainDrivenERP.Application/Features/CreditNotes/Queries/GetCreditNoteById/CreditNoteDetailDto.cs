using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Features.CreditNotes.Queries.GetCreditNoteById;

public sealed record CreditNoteDetailDto(
    Guid Id,
    Guid CustomerId,
    Guid CompanyId,
    string? SequenceNumber,
    string Reason,
    DateTime NoteDate,
    string Status,
    decimal TotalAmount,
    decimal TotalTax,
    decimal GrandTotal,
    IReadOnlyList<CreditNoteLineDto> Lines);

public sealed record CreditNoteLineDto(
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
    IReadOnlyList<CreditLineTaxBreakdownDto> TaxBreakdowns,
    IReadOnlyList<CreditLineDiscountBreakdownDto> DiscountBreakdowns);

public sealed record CreditLineTaxBreakdownDto(
    string TaxCode,
    string TaxName,
    decimal Rate,
    decimal TaxAmount,
    bool IsWithholding);

public sealed record CreditLineDiscountBreakdownDto(
    string Source,
    string Type,
    decimal DiscountValue,
    decimal DiscountAmount);
