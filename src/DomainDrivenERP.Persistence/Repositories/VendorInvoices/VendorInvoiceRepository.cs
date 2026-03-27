using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.VendorInvoices;

internal sealed class VendorInvoiceRepository : IVendorInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public VendorInvoiceRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<VendorInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.VendorInvoices
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<VendorInvoice?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
        => await _context.VendorInvoices
            .Include(x => x.Lines)
                .ThenInclude(l => l.TaxBreakdowns)
            .Include(x => x.Lines)
                .ThenInclude(l => l.DiscountBreakdowns)
            .Include(x => x.InvoiceDiscounts)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<VendorInvoice>> GetByVendorIdAsync(
        Guid vendorId, CancellationToken ct = default)
        => await _context.VendorInvoices
            .Where(x => x.VendorId == vendorId)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<VendorInvoice>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _context.VendorInvoices
            .Where(x => x.Status == status && x.CompanyId == companyId)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(ct);

    public async Task AddAsync(VendorInvoice invoice, CancellationToken ct = default)
        => await _context.VendorInvoices.AddAsync(invoice, ct);

    public Task UpdateAsync(VendorInvoice invoice, CancellationToken ct = default)
    {
        _context.VendorInvoices.Update(invoice);
        return Task.CompletedTask;
    }

    public async Task<bool> IsVendorInvoiceNumberDuplicateAsync(
        string vendorInvoiceNumber, Guid vendorId, CancellationToken ct = default)
        => await _context.VendorInvoices
            .AnyAsync(x => x.VendorInvoiceNumber == vendorInvoiceNumber && x.VendorId == vendorId, ct);
}
