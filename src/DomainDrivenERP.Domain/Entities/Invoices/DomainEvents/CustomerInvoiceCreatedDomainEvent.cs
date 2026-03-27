using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;

public sealed record CustomerInvoiceCreatedDomainEvent(
    Guid InvoiceId,
    Guid CustomerId,
    Guid CompanyId) : DomainEvent(Guid.NewGuid());
