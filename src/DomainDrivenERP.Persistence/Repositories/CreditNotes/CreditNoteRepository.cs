using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.CreditNotes;

internal sealed class CreditNoteRepository : ICreditNoteRepository
{
    private readonly ApplicationDbContext _context;

    public CreditNoteRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<CreditNote?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.CreditNotes
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<CreditNote?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
        => await _context.CreditNotes
            .Include(x => x.Lines)
                .ThenInclude(l => l.TaxBreakdowns)
            .Include(x => x.Lines)
                .ThenInclude(l => l.DiscountBreakdowns)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<CreditNote>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken ct = default)
        => await _context.CreditNotes
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.NoteDate)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CreditNote>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _context.CreditNotes
            .Where(x => x.Status == status && x.CompanyId == companyId)
            .OrderByDescending(x => x.NoteDate)
            .ToListAsync(ct);

    public async Task AddAsync(CreditNote note, CancellationToken ct = default)
        => await _context.CreditNotes.AddAsync(note, ct);

    public Task UpdateAsync(CreditNote note, CancellationToken ct = default)
    {
        _context.CreditNotes.Update(note);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _context.CreditNotes.AnyAsync(x => x.Id == id, ct);
}
