using DomainDrivenERP.Domain.Entities.TaxGroups;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface ITaxGroupRepository
{
    Task<TaxGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaxGroup?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaxGroup?> GetDefaultByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<List<TaxGroup>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task AddAsync(TaxGroup taxGroup);
    Task Update(TaxGroup taxGroup);
}
