using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.TaxDefinitions.DomainEvents;

public sealed record TaxDefinitionCreatedDomainEvent(Guid TaxDefinitionId) : DomainEvent(Guid.NewGuid())
{
}
