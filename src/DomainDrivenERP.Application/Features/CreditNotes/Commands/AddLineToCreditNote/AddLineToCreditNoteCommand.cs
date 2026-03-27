using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.AddLineToCreditNote;

public sealed record AddLineToCreditNoteCommand(
    Guid NoteId,
    Guid ItemId,
    decimal Quantity,
    string QuantityUnit,
    decimal UnitPrice,
    string Currency,
    Guid? TaxGroupId = null,
    Guid? DiscountGroupId = null,
    int SortOrder = 0) : ICommand<AddLineToCreditNoteResult>;

public sealed record AddLineToCreditNoteResult(
    Guid LineId,
    Guid NoteId,
    Guid ItemId,
    decimal SubTotal,
    decimal TotalDiscountAmount,
    decimal TotalTaxAmount,
    decimal FinalLineTotal);
