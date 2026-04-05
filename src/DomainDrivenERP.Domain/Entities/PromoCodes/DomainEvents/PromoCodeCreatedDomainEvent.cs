using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.PromoCodes.DomainEvents;

public sealed record PromoCodeCreatedDomainEvent(Guid PromoCodeId, Guid CompanyId, string Code) : DomainEvent(Guid.NewGuid())
{
}
