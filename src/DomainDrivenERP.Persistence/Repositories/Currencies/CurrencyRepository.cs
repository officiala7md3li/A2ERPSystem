using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.Currencies;

internal class CurrencyRepository : ICurrencyRepository
{
    private readonly ApplicationDbContext _context;

    public CurrencyRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Currency currency)
        => await _context.Currencies.AddAsync(currency);

    public async Task<Currency?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Currency?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _context.Currencies.FirstOrDefaultAsync(c => c.Code == code, ct);

    public async Task<Currency?> GetBaseCurrencyAsync(CancellationToken ct = default)
        => await _context.Currencies.FirstOrDefaultAsync(c => c.IsBase, ct);

    public async Task<List<Currency>> GetAllAsync(CancellationToken ct = default)
        => await _context.Currencies.OrderBy(c => c.Code).ToListAsync(ct);

    public Task Update(Currency currency)
    {
        _context.Currencies.Update(currency);
        return Task.CompletedTask;
    }
}
