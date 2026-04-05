using DomainDrivenERP.Domain.Entities.DiscountGroups;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IDiscountGroupRepository
{
    Task<DiscountGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DiscountGroup?> GetWithRulesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DiscountGroup>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task AddAsync(DiscountGroup discountGroup);
    Task Update(DiscountGroup discountGroup);
}
