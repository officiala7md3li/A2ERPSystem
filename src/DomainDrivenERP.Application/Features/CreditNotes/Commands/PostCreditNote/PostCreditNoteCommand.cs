using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.PostCreditNote;

public sealed record PostCreditNoteCommand(Guid NoteId) : ICommand<PostCreditNoteResult>;

public sealed record PostCreditNoteResult(
    Guid NoteId,
    string SequenceNumber,
    decimal GrandTotal,
    string Status);
