using DomainDrivenERP.Application.Abstractions.Messaging;
using System;

namespace DomainDrivenERP.Application.Features.CreditNotes.Queries.GetCreditNoteById;

public sealed record GetCreditNoteByIdQuery(Guid NoteId) : IQuery<CreditNoteDetailDto>;
