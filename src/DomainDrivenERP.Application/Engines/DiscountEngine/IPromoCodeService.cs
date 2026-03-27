using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Engines.DiscountEngine;

/// <summary>
/// Validates a promo code against usage limits using Redis INCR first; falls back to a DB row-level lock.
/// Returns the resolved discount value if valid.
/// </summary>
public interface IPromoCodeService
{
    /// <summary>
    /// Attempts to consume one use of the promo code. Returns the discount value (as a %) on success.
    /// Returns null if the code is invalid, expired, or at its usage limit.
    /// </summary>
    Task<decimal?> TryConsumeAsync(
        string code,
        Guid customerId,
        decimal orderAmount,
        CancellationToken ct = default);
}
