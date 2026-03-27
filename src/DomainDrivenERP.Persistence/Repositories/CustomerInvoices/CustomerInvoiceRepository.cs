using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.CustomerInvoices;

internal sealed class CustomerInvoiceRepository : ICustomerInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerInvoiceRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<CustomerInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.CustomerInvoices
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<CustomerInvoice?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
        => await _context.CustomerInvoices
            .Include(x => x.Lines)
                .ThenInclude(l => l.TaxBreakdowns)
            .Include(x => x.Lines)
                .ThenInclude(l => l.DiscountBreakdowns)
            .Include(x => x.InvoiceDiscounts)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<CustomerInvoice?> GetByIdWithLinesNoTrackingAsync(Guid id, CancellationToken ct = default)
        => await _context.CustomerInvoices
            .AsNoTracking()
            .Include(x => x.Lines)
            .Include(x => x.InvoiceDiscounts)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<CustomerInvoice>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken ct = default)
        => await _context.CustomerInvoices
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CustomerInvoice>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _context.CustomerInvoices
            .Where(x => x.Status == status && x.CompanyId == companyId)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(ct);

    public async Task AddAsync(CustomerInvoice invoice, CancellationToken ct = default)
        => await _context.CustomerInvoices.AddAsync(invoice, ct);

    public async Task AddLineAsync(InvoiceLine line, CancellationToken ct = default)
        => await _context.Set<InvoiceLine>().AddAsync(line, ct);

    public async Task AddBreakdownsAsync(IEnumerable<LineTaxBreakdown> taxes, IEnumerable<LineDiscountBreakdown> discounts, CancellationToken ct = default)
    {
        if (taxes.Any())
            await _context.Set<LineTaxBreakdown>().AddRangeAsync(taxes, ct);
        if (discounts.Any())
            await _context.Set<LineDiscountBreakdown>().AddRangeAsync(discounts, ct);
    }

    public Task UpdateAsync(CustomerInvoice invoice, CancellationToken ct = default)
    {
        _context.CustomerInvoices.Update(invoice);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _context.CustomerInvoices.AnyAsync(x => x.Id == id, ct);

    public async Task<bool> IsSequenceNumberUniqueAsync(
        string sequenceNumber, Guid companyId, CancellationToken ct = default)
        => !await _context.CustomerInvoices
            .AnyAsync(x => x.SequenceNumber == sequenceNumber && x.CompanyId == companyId, ct);
}
