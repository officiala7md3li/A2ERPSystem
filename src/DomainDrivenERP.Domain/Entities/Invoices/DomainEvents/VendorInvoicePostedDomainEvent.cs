using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Invoices.DomainEvents;

public sealed record VendorInvoicePostedDomainEvent(
    Guid InvoiceId,
    Guid VendorId,
    Guid CompanyId,
    decimal GrandTotal) : DomainEvent(Guid.NewGuid());
