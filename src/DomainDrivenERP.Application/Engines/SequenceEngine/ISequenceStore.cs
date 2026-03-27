using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Engines.SequenceEngine;

/// <summary>
/// Persistence abstraction for the Sequence Engine. 
/// Implementation lives in DomainDrivenERP.Persistence.
/// </summary>
public interface ISequenceStore
{
    /// <summary>
    /// Returns the current counter for a prefix/company/date, or 0 if none exists yet.
    /// Used to seed Redis on first access.
    /// </summary>
    Task<long> GetCurrentCounterAsync(
        string prefix,
        Guid companyId,
        DateTime date,
        CancellationToken ct = default);

    /// <summary>
    /// Saves the Redis-provided counter value to the DB (non-locking, best-effort).
    /// </summary>
    Task IncrementAsync(
        string prefix,
        Guid companyId,
        DateTime date,
        long counterValue,
        CancellationToken ct = default);

    /// <summary>
    /// Pessimistic row-lock increment — used ONLY as Redis fallback.
    /// Acquires SELECT ... WITH (UPDLOCK) and increments atomically.
    /// </summary>
    Task<long> IncrementWithLockAsync(
        string prefix,
        Guid companyId,
        DateTime date,
        CancellationToken ct = default);
}
