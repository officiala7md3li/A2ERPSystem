using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.COAs.DomainEvents;
public sealed record CreateFirstLevelCoaDomainEvent(string HeadName, ChartOfAccountsType Type) : DomainEvent(Guid.NewGuid());
