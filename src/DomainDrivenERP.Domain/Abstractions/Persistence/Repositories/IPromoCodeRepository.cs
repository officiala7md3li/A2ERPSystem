using DomainDrivenERP.Domain.Entities.PromoCodes;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IPromoCodeRepository
{
    Task<PromoCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PromoCode?> GetByCodeAsync(string code, Guid companyId, CancellationToken cancellationToken = default);
    Task<PromoCode?> GetWithUsagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PromoCode>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<List<PromoCode>> GetActiveByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task AddAsync(PromoCode promoCode);
    Task Update(PromoCode promoCode);
}
