using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.PriceLists.DomainEvents;

public sealed record PriceListCreatedDomainEvent(Guid PriceListId, Guid CompanyId, string NameEn) : DomainEvent(Guid.NewGuid())
{
}
