using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;

public sealed record VendorInvoiceCreatedDomainEvent(
    Guid InvoiceId,
    Guid VendorId,
    Guid CompanyId) : DomainEvent(Guid.NewGuid());
