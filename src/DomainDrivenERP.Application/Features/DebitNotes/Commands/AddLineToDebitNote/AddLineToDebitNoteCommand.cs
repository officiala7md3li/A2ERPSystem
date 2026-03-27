using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.AddLineToDebitNote;

public sealed record AddLineToDebitNoteCommand(
    Guid NoteId,
    Guid ItemId,
    decimal Quantity,
    string QuantityUnit,
    decimal UnitPrice,
    string Currency,
    Guid? TaxGroupId = null,
    Guid? DiscountGroupId = null,
    int SortOrder = 0) : ICommand<AddLineToDebitNoteResult>;

public sealed record AddLineToDebitNoteResult(
    Guid LineId,
    Guid NoteId,
    Guid ItemId,
    decimal SubTotal,
    decimal TotalDiscountAmount,
    decimal TotalTaxAmount,
    decimal FinalLineTotal);
