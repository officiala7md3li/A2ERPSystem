using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.PostDebitNote;

public sealed record PostDebitNoteCommand(Guid NoteId) : ICommand<PostDebitNoteResult>;

public sealed record PostDebitNoteResult(
    Guid NoteId,
    string SequenceNumber,
    decimal GrandTotal,
    string Status);
