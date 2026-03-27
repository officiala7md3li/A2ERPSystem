using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Enums;
using System;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.CreateDebitNote;

public sealed record CreateDebitNoteCommand(
    Guid CustomerId,
    Guid CompanyId,
    Guid CurrencyId,
    DateTime NoteDate,
    Guid OriginalInvoiceId,
    string Reason) : ICommand<CreateDebitNoteResult>;

public sealed record CreateDebitNoteResult(
    Guid NoteId,
    Guid CustomerId,
    DateTime NoteDate,
    string Status);
