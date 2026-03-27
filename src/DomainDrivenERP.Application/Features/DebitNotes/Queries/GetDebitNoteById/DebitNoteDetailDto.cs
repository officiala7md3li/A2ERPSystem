using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Application.Features.DebitNotes.Queries.GetDebitNoteById;

public sealed record DebitNoteDetailDto(
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
    IReadOnlyList<DebitNoteLineDto> Lines);

public sealed record DebitNoteLineDto(
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
    IReadOnlyList<DebitLineTaxBreakdownDto> TaxBreakdowns,
    IReadOnlyList<DebitLineDiscountBreakdownDto> DiscountBreakdowns);

public sealed record DebitLineTaxBreakdownDto(
    string TaxCode,
    string TaxName,
    decimal Rate,
    decimal TaxAmount,
    bool IsWithholding);

public sealed record DebitLineDiscountBreakdownDto(
    string Source,
    string Type,
    decimal DiscountValue,
    decimal DiscountAmount);
