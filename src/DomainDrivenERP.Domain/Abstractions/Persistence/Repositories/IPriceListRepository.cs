using DomainDrivenERP.Domain.Entities.PriceLists;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IPriceListRepository
{
    Task<PriceList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PriceList?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PriceList?> GetForCustomerAsync(Guid customerId, Guid companyId, DateTime date, CancellationToken cancellationToken = default);
    Task<PriceList?> GetDefaultByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<List<PriceList>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task AddAsync(PriceList priceList);
    Task Update(PriceList priceList);
}
