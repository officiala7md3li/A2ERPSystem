using DomainDrivenERP.Domain.Entities.UnitOfMeasures;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;

public interface IUnitOfMeasureRepository
{
    Task<UnitOfMeasure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UnitOfMeasure?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<UnitOfMeasure>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<UnitOfMeasure>> GetByTypeAsync(UomType type, CancellationToken cancellationToken = default);
    Task AddAsync(UnitOfMeasure unitOfMeasure);
    Task Update(UnitOfMeasure unitOfMeasure);
}
