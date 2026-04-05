using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Vendors.DomainEvents;

public sealed record VendorCreatedDomainEvent(Guid VendorId, Guid CompanyId, string NameEn) : DomainEvent(Guid.NewGuid())
{
}
