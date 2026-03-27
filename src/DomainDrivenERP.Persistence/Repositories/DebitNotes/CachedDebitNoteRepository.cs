using DomainDrivenERP.Domain.Abstractions.Persistence.Caching;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Persistence.Repositories.DebitNotes;

internal sealed class CachedDebitNoteRepository : IDebitNoteRepository
{
    private readonly IDebitNoteRepository _decorated;
    private readonly ICacheService _cache;

    private static readonly TimeSpan ListExpiry = TimeSpan.FromMinutes(2);

    public CachedDebitNoteRepository(
        IDebitNoteRepository decorated,
        ICacheService cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<DebitNote?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _decorated.GetByIdAsync(id, ct);

    public async Task<DebitNote?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
        => await _decorated.GetByIdWithLinesAsync(id, ct);

    public async Task<IReadOnlyList<DebitNote>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken ct = default)
    {
        string key = $"debit-notes:{customerId}";
        return await _cache.GetOrSetAsync(
            key,
            () => _decorated.GetByCustomerIdAsync(customerId, ct),
            ct,
            ListExpiry) ?? new List<DebitNote>();
    }

    public async Task<IReadOnlyList<DebitNote>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _decorated.GetByStatusAsync(status, companyId, ct);

    public async Task AddAsync(DebitNote note, CancellationToken ct = default)
    {
        await _decorated.AddAsync(note, ct);
        await InvalidateCustomerCacheAsync(note.CustomerId, ct);
    }

    public async Task UpdateAsync(DebitNote note, CancellationToken ct = default)
    {
        await _decorated.UpdateAsync(note, ct);
        await InvalidateCustomerCacheAsync(note.CustomerId, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _decorated.ExistsAsync(id, ct);

    private async Task InvalidateCustomerCacheAsync(Guid customerId, CancellationToken ct)
        => await _cache.RemoveAsync($"debit-notes:{customerId}", ct);
}
