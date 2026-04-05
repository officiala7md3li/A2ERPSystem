using DomainDrivenERP.Domain.Primitives;

namespace DomainDrivenERP.Domain.Entities.PromoCodes.DomainEvents;

public sealed record PromoCodeUsedDomainEvent(Guid PromoCodeId, Guid CustomerId, Guid InvoiceId, decimal DiscountApplied) : DomainEvent(Guid.NewGuid())
{
}
