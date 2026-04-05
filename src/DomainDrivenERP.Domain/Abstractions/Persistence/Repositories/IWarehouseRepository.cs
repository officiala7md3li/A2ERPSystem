using DomainDrivenERP.Domain.Entities.Warehouses;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetByCodeAsync(string code, Guid companyId, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetWithSubWarehousesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Warehouse>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<List<Warehouse>> GetMainWarehousesAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetDefaultForSalesAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetDefaultForPurchasesAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task AddAsync(Warehouse warehouse);
    Task Update(Warehouse warehouse);
}
