using DomainDrivenERP.Domain.Entities.Currencies;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface ICurrencyRepository
{
    Task<Currency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Currency?> GetBaseCurrencyAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Currency currency);
    Task Update(Currency currency);
}
