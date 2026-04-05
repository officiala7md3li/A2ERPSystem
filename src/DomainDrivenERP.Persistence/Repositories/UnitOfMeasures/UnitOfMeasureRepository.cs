using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.UnitOfMeasures;

internal class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly ApplicationDbContext _context;

    public UnitOfMeasureRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(UnitOfMeasure uom)
        => await _context.UnitOfMeasures.AddAsync(uom);

    public async Task<UnitOfMeasure?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.UnitOfMeasures.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<UnitOfMeasure?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _context.UnitOfMeasures.FirstOrDefaultAsync(u => u.Code == code, ct);

    public async Task<List<UnitOfMeasure>> GetAllAsync(CancellationToken ct = default)
        => await _context.UnitOfMeasures.OrderBy(u => u.Code).ToListAsync(ct);

    public async Task<List<UnitOfMeasure>> GetByTypeAsync(UomType type, CancellationToken ct = default)
        => await _context.UnitOfMeasures.Where(u => u.Type == type).OrderBy(u => u.Code).ToListAsync(ct);

    public Task Update(UnitOfMeasure uom)
    {
        _context.UnitOfMeasures.Update(uom);
        return Task.CompletedTask;
    }
}
