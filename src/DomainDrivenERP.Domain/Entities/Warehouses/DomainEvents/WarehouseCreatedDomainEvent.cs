using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.Warehouses.DomainEvents;

public sealed record WarehouseCreatedDomainEvent(Guid WarehouseId, Guid CompanyId, string NameEn, bool IsMain) : DomainEvent(Guid.NewGuid())
{
}
