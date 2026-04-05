using DomainDrivenERP.Domain.Entities.TaxDefinitions;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface ITaxDefinitionRepository
{
    Task<TaxDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaxDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<TaxDefinition>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<TaxDefinition?> GetWithDependenciesAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaxDefinition taxDefinition);
    Task Update(TaxDefinition taxDefinition);
}
