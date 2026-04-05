using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.DiscountGroups.DomainEvents;

public sealed record DiscountGroupCreatedDomainEvent(Guid DiscountGroupId, Guid CompanyId, string NameEn) : DomainEvent(Guid.NewGuid())
{
}
