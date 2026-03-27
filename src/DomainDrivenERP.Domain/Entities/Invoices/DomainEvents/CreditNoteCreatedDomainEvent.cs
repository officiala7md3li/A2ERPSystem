using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;

public sealed record CreditNoteCreatedDomainEvent(
    Guid NoteId,
    Guid CustomerId,
    Guid OriginalInvoiceId) : DomainEvent(Guid.NewGuid());
