using DomainDrivenERP.Domain.Abstractions.Persistence.Caching;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;

namespace DomainDrivenERP.Persistence.Repositories.CustomerInvoices;

internal sealed class CachedCustomerInvoiceRepository : ICustomerInvoiceRepository
{
    private readonly ICustomerInvoiceRepository _decorated;
    private readonly ICacheService _cache;

    // Cache Expiration Settings
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ListExpiry = TimeSpan.FromMinutes(2);

    public CachedCustomerInvoiceRepository(
        ICustomerInvoiceRepository decorated,
        ICacheService cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<CustomerInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // Draft Invoices مش بنحطهاش في Cache لأنها بتتغير كتير
        return await _decorated.GetByIdAsync(id, ct);
    }

    public async Task<CustomerInvoice?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
    {
        // Full Invoice مع Lines مش بنـ Cache بسبب التعقيد والتغيير المستمر
        return await _decorated.GetByIdWithLinesAsync(id, ct);
    }

    public Task<CustomerInvoice?> GetByIdWithLinesNoTrackingAsync(Guid id, CancellationToken ct = default)
        => _decorated.GetByIdWithLinesNoTrackingAsync(id, ct);

    public async Task<IReadOnlyList<CustomerInvoice>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken ct = default)
    {
        // List بنـ Cache لمدة قصيرة للـ Performance
        string key = $"customer-invoices:{customerId}";
        return await _cache.GetOrSetAsync(
            key,
            () => _decorated.GetByCustomerIdAsync(customerId, ct),
            ct,
            ListExpiry) ?? new List<CustomerInvoice>();
    }

    public async Task<IReadOnlyList<CustomerInvoice>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _decorated.GetByStatusAsync(status, companyId, ct);

    public async Task AddAsync(CustomerInvoice invoice, CancellationToken ct = default)
    {
        await _decorated.AddAsync(invoice, ct);
        await InvalidateCustomerCacheAsync(invoice.CustomerId, ct);
    }

    public async Task AddLineAsync(InvoiceLine line, CancellationToken ct = default)
    {
        await _decorated.AddLineAsync(line, ct);
        // لا نحتاج لمسح الكاش هنا إذا كنا لا نخزن الفاتورة بخطوطها، لكن للأمان:
        await _cache.RemoveAsync($"CustomerInvoice-{line.InvoiceId}", ct);
    }

    public async Task AddBreakdownsAsync(IEnumerable<LineTaxBreakdown> taxes, IEnumerable<LineDiscountBreakdown> discounts, CancellationToken ct = default)
    {
        await _decorated.AddBreakdownsAsync(taxes, discounts, ct);
    }

    public async Task UpdateAsync(CustomerInvoice invoice, CancellationToken ct = default)
    {
        await _decorated.UpdateAsync(invoice, ct);
        await InvalidateCustomerCacheAsync(invoice.CustomerId, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _decorated.ExistsAsync(id, ct);

    public Task<bool> IsSequenceNumberUniqueAsync(
        string sequenceNumber, Guid companyId, CancellationToken ct = default)
        => _decorated.IsSequenceNumberUniqueAsync(sequenceNumber, companyId, ct);

    private async Task InvalidateCustomerCacheAsync(Guid customerId, CancellationToken ct)
        => await _cache.RemoveAsync($"customer-invoices:{customerId}", ct);
}
