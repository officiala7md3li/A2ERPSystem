using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.SubmitDebitNote;

public sealed record SubmitDebitNoteCommand(Guid NoteId) : ICommand;
