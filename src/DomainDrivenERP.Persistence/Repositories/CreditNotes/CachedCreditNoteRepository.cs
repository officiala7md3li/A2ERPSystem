using DomainDrivenERP.Domain.Abstractions.Persistence.Caching;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Persistence.Repositories.CreditNotes;

internal sealed class CachedCreditNoteRepository : ICreditNoteRepository
{
    private readonly ICreditNoteRepository _decorated;
    private readonly ICacheService _cache;

    private static readonly TimeSpan ListExpiry = TimeSpan.FromMinutes(2);

    public CachedCreditNoteRepository(
        ICreditNoteRepository decorated,
        ICacheService cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<CreditNote?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _decorated.GetByIdAsync(id, ct);

    public async Task<CreditNote?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
        => await _decorated.GetByIdWithLinesAsync(id, ct);

    public async Task<IReadOnlyList<CreditNote>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken ct = default)
    {
        string key = $"credit-notes:{customerId}";
        return await _cache.GetOrSetAsync(
            key,
            () => _decorated.GetByCustomerIdAsync(customerId, ct),
            ct,
            ListExpiry) ?? new List<CreditNote>();
    }

    public async Task<IReadOnlyList<CreditNote>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _decorated.GetByStatusAsync(status, companyId, ct);

    public async Task AddAsync(CreditNote note, CancellationToken ct = default)
    {
        await _decorated.AddAsync(note, ct);
        await InvalidateCustomerCacheAsync(note.CustomerId, ct);
    }

    public async Task UpdateAsync(CreditNote note, CancellationToken ct = default)
    {
        await _decorated.UpdateAsync(note, ct);
        await InvalidateCustomerCacheAsync(note.CustomerId, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _decorated.ExistsAsync(id, ct);

    private async Task InvalidateCustomerCacheAsync(Guid customerId, CancellationToken ct)
        => await _cache.RemoveAsync($"credit-notes:{customerId}", ct);
}
