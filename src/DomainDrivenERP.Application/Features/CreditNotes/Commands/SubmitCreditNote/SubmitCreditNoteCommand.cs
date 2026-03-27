using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.SubmitCreditNote;

public sealed record SubmitCreditNoteCommand(Guid NoteId) : ICommand;
