using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;

public sealed record CustomerInvoicePostedDomainEvent(
    Guid InvoiceId,
    Guid CustomerId,
    Guid CompanyId,
    decimal GrandTotal) : DomainEvent(Guid.NewGuid());
