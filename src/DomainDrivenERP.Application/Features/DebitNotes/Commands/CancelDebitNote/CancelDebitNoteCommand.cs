using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.CancelDebitNote;

public sealed record CancelDebitNoteCommand(Guid NoteId, string Reason) : ICommand;
