using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.Companies;

internal class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Company company)
        => await _context.Companies.AddAsync(company);

    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<Company>> GetAllAsync(CancellationToken ct = default)
        => await _context.Companies.OrderBy(c => c.NameEn).ToListAsync(ct);

    public Task Update(Company company)
    {
        _context.Companies.Update(company);
        return Task.CompletedTask;
    }
}
