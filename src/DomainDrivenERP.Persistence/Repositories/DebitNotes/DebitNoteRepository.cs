using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.DebitNotes;

internal sealed class DebitNoteRepository : IDebitNoteRepository
{
    private readonly ApplicationDbContext _context;

    public DebitNoteRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<DebitNote?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.DebitNotes
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<DebitNote?> GetByIdWithLinesAsync(Guid id, CancellationToken ct = default)
        => await _context.DebitNotes
            .Include(x => x.Lines)
                .ThenInclude(l => l.TaxBreakdowns)
            .Include(x => x.Lines)
                .ThenInclude(l => l.DiscountBreakdowns)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<DebitNote>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken ct = default)
        => await _context.DebitNotes
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.NoteDate)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DebitNote>> GetByStatusAsync(
        InvoiceStatus status, Guid companyId, CancellationToken ct = default)
        => await _context.DebitNotes
            .Where(x => x.Status == status && x.CompanyId == companyId)
            .OrderByDescending(x => x.NoteDate)
            .ToListAsync(ct);

    public async Task AddAsync(DebitNote note, CancellationToken ct = default)
        => await _context.DebitNotes.AddAsync(note, ct);

    public Task UpdateAsync(DebitNote note, CancellationToken ct = default)
    {
        _context.DebitNotes.Update(note);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _context.DebitNotes.AnyAsync(x => x.Id == id, ct);
}
