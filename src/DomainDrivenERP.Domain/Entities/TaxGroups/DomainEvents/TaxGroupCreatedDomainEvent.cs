using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.TaxGroups.DomainEvents;

public sealed record TaxGroupCreatedDomainEvent(Guid TaxGroupId, Guid CompanyId, string NameEn) : DomainEvent(Guid.NewGuid())
{
}
