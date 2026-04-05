using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Currencies.DomainEvents;

public sealed record CurrencyCreatedDomainEvent(Guid CurrencyId) : DomainEvent(Guid.NewGuid())
{
}
