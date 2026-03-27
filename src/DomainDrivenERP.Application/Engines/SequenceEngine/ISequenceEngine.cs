using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Engines.SequenceEngine;

/// <summary>
/// Generates concurrency-safe sequential document numbers.
/// </summary>
public interface ISequenceEngine
{
    /// <summary>
    /// Generates the next formatted sequence number for a given document type prefix.
    /// Acquires a distributed lock to ensure no duplicates are ever issued, even under high concurrency.
    /// </summary>
    /// <param name="prefix">Document type prefix, e.g. "INV", "VND", "CRN", "DBN".</param>
    /// <param name="companyId">Isolates sequences per company.</param>
    /// <param name="date">Date used for formatting. Defaults to today (UTC).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Formatted sequence number, e.g. "INV-20260324-000042".</returns>
    Task<string> NextAsync(
        string prefix,
        Guid companyId,
        DateTime? date = null,
        CancellationToken ct = default);
}
