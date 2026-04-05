using DomainDrivenERP.Domain.Entities.Companies;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Company>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Company company);
    Task Update(Company company);
}
