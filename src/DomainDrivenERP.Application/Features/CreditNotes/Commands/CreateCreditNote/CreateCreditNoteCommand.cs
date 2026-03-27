using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Enums;
using System;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.CreateCreditNote;

public sealed record CreateCreditNoteCommand(
    Guid CustomerId,
    Guid CompanyId,
    Guid CurrencyId,
    DateTime NoteDate,
    Guid OriginalInvoiceId,
    string Reason) : ICommand<CreateCreditNoteResult>;

public sealed record CreateCreditNoteResult(
    Guid NoteId,
    Guid CustomerId,
    DateTime NoteDate,
    string Status);
