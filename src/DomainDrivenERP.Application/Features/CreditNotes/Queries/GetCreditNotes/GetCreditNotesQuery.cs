using DomainDrivenERP.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace DomainDrivenERP.Application.Features.CreditNotes.Queries.GetCreditNotes;

public sealed record GetCreditNotesQuery(Guid CustomerId) : IQuery<IReadOnlyList<CreditNoteListDto>>;

public sealed record CreditNoteListDto(
    Guid Id,
    Guid CustomerId,
    string? SequenceNumber,
    string Reason,
    DateTime InvoiceDate,
    string Status,
    decimal GrandTotal);
