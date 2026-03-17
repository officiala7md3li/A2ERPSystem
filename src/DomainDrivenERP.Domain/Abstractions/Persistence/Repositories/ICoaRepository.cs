using DomainDrivenERP.Domain.Entities.COAs;

namespace DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
public interface ICoaRepository
{
    Task CreateCoa(Accounts cOA, CancellationToken cancellationToken = default);
    Task<Accounts?> GetCoaById(Guid coaId, CancellationToken cancellationToken = default);
    Task<Accounts?> GetCoaByName(string coaParentName, CancellationToken cancellationToken = default);
    Task<List<Accounts>?> GetCoaChilds(Guid parentAccountId, CancellationToken cancellationToken = default);
    Task<bool> IsCoaExist(Guid coaId, CancellationToken cancellationToken = default);
    Task<bool> IsCoaExist(string coaName, int level = 1, CancellationToken cancellationToken = default);
    Task<bool> IsCoaExist(string coaName, string coaParentName, CancellationToken cancellationToken = default);
    Task<Accounts?> GetCoaWithChildren(Guid coaId, CancellationToken cancellationToken = default);
    Task<string?> GetLastHeadCodeInLevelOne(CancellationToken cancellationToken = default);
    Task<Guid?> GetByAccountName(string accountName, CancellationToken cancellationToken = default);
    Task<Guid?> GetByAccountHeadCode(string accountHeadCode, CancellationToken cancellationToken = default);
}
