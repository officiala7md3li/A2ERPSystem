using DomainDrivenERP.Domain.Abstractions.Persistence.Caching;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Persistence.Repositories.VendorInvoices;

internal sealed class CachedVendorInvoiceRepository : IVendorInvoiceRepository
{
    private readonly IVendorInvoiceRepository _decorated;
    private readonly ICacheService _cache;

    private static readonly TimeSpan ListExpiry = TimeSpan.FromMinutes(2);

    public CachedVendorInvoiceRepository(
        IVendorInvoiceRepository decorated,
        ICacheService cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<VendorInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _decorated.GetByIdAsync(id, ct);

    public async Task<VendorInvoice?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
        => await _decorated.GetByIdWithLinesAsync(id, ct);

    public async Task<IReadOnlyList<VendorInvoice>> GetByVendorIdAsync(
        Guid vendorId, CancellationToken ct = default)
    {
        string key = $"vendor-invoices:{vendorId}";
        return await _cache.GetOrSetAsync(
            key,
            () => _decorated.GetByVendorIdAsync(vendorId, ct),
            ct,
            ListExpiry) ?? new List<VendorInvoice>();
    }

    public async Task<IReadOnlyList<VendorInvoice>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _decorated.GetByStatusAsync(status, companyId, ct);

    public async Task AddAsync(VendorInvoice invoice, CancellationToken ct = default)
    {
        await _decorated.AddAsync(invoice, ct);
        await InvalidateVendorCacheAsync(invoice.VendorId, ct);
    }

    public async Task UpdateAsync(VendorInvoice invoice, CancellationToken ct = default)
    {
        await _decorated.UpdateAsync(invoice, ct);
        await InvalidateVendorCacheAsync(invoice.VendorId, ct);
    }

    public Task<bool> IsVendorInvoiceNumberDuplicateAsync(
        string vendorInvoiceNumber, Guid vendorId, CancellationToken ct = default)
        => _decorated.IsVendorInvoiceNumberDuplicateAsync(vendorInvoiceNumber, vendorId, ct);

    private async Task InvalidateVendorCacheAsync(Guid vendorId, CancellationToken ct)
        => await _cache.RemoveAsync($"vendor-invoices:{vendorId}", ct);
}
