using System;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Engines.SequenceEngine;

/// <summary>
/// Concurrency-safe sequence number generator.
/// Delegates entirely to ISequenceStore, which handles Redis-first and DB-fallback internally.
/// Format: {PREFIX}-{YYYYMMDD}-{COUNTER:D6}
/// </summary>
public sealed class SequenceEngine : ISequenceEngine
{
    private readonly ISequenceStore _store;

    public SequenceEngine(ISequenceStore store)
    {
        _store = store;
    }

    public async Task<string> NextAsync(
        string prefix,
        Guid companyId,
        DateTime? date = null,
        CancellationToken ct = default)
    {
        var effectiveDate = (date ?? DateTime.UtcNow).Date;
        var datePart = effectiveDate.ToString("yyyyMMdd");

        // ISequenceStore implementation handles Redis + DB fallback internally
        var counter = await _store.IncrementWithLockAsync(prefix, companyId, effectiveDate, ct);

        return $"{prefix}-{datePart}-{counter:D6}";
    }
}
