using DomainDrivenERP.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace DomainDrivenERP.Application.Features.DebitNotes.Queries.GetDebitNotes;

public sealed record GetDebitNotesQuery(Guid CustomerId) : IQuery<IReadOnlyList<DebitNoteListDto>>;

public sealed record DebitNoteListDto(
    Guid Id,
    Guid CustomerId,
    string? SequenceNumber,
    string Reason,
    DateTime InvoiceDate,
    string Status,
    decimal GrandTotal);
