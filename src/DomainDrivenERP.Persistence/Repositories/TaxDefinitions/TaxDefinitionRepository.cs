using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.TaxDefinitions;

internal class TaxDefinitionRepository : ITaxDefinitionRepository
{
    private readonly ApplicationDbContext _context;

    public TaxDefinitionRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(TaxDefinition taxDef)
        => await _context.TaxDefinitions.AddAsync(taxDef);

    public async Task<TaxDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.TaxDefinitions.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<TaxDefinition?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _context.TaxDefinitions.FirstOrDefaultAsync(t => t.Code == code, ct);

    public async Task<List<TaxDefinition>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.TaxDefinitions.Where(t => t.IsActive).OrderBy(t => t.Code).ToListAsync(ct);

    public async Task<TaxDefinition?> GetWithDependenciesAsync(Guid id, CancellationToken ct = default)
        => await _context.TaxDefinitions
            .Include(t => t.Dependencies)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task Update(TaxDefinition taxDef)
    {
        _context.TaxDefinitions.Update(taxDef);
        return Task.CompletedTask;
    }
}
