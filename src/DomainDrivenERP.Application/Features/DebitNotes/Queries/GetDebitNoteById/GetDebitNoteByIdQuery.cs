using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.DebitNotes.Queries.GetDebitNoteById;

public sealed record GetDebitNoteByIdQuery(Guid NoteId) : IQuery<DebitNoteDetailDto>;
