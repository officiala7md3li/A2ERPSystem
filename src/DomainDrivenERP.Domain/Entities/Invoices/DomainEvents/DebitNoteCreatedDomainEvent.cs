using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;

public sealed record DebitNoteCreatedDomainEvent(
    Guid NoteId,
    Guid CustomerId,
    Guid OriginalInvoiceId) : DomainEvent(Guid.NewGuid());
