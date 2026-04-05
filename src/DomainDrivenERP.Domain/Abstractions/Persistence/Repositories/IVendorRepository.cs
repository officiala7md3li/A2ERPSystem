using DomainDrivenERP.Domain.Entities.Vendors;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IVendorRepository
{
    Task<Vendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Vendor>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<Vendor?> GetByTaxNumberAsync(string taxRegistrationNumber, Guid companyId, CancellationToken cancellationToken = default);
    Task<List<Vendor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Vendor vendor);
    Task Update(Vendor vendor);
}
