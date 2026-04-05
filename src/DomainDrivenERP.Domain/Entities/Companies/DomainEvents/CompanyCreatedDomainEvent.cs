using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Companies.DomainEvents;

public sealed record CompanyCreatedDomainEvent(Guid CompanyId) : DomainEvent(Guid.NewGuid())
{
}
