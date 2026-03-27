using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.CancelCreditNote;

public sealed record CancelCreditNoteCommand(Guid NoteId, string Reason) : ICommand;
