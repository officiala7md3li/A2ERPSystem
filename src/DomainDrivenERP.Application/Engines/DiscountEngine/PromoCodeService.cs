using System;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Engines.DiscountEngine;

/// <summary>
/// Validates a promo code against usage limits using Redis INCR first; falls back to a DB row-level lock.
/// The concrete implementation lives in the Infrastructure layer.
/// </summary>
public sealed class PromoCodeService : IPromoCodeService
{
    public Task<decimal?> TryConsumeAsync(
        string code,
        Guid customerId,
        decimal orderAmount,
        CancellationToken ct = default)
    {
        throw new NotSupportedException(
            "PromoCodeService must be implemented at the Infrastructure layer. " +
            "Register an IPromoCodeService implementation in InfrastructureDependencies.cs.");
    }
}
