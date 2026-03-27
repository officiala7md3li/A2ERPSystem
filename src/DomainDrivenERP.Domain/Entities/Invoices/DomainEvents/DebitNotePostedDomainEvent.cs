using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;

public sealed record DebitNotePostedDomainEvent(
    Guid NoteId,
    Guid CustomerId,
    Guid OriginalInvoiceId,
    decimal GrandTotal) : DomainEvent(Guid.NewGuid());
