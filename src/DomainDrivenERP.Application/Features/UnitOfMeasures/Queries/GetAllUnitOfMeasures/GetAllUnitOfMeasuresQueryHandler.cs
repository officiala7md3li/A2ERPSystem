using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.UnitOfMeasures.Queries.GetAllUnitOfMeasures;

public class GetAllUnitOfMeasuresQueryHandler : IQueryHandler<GetAllUnitOfMeasuresQuery, List<UnitOfMeasure>>
{
    private readonly IUnitOfMeasureRepository _repo;
    public GetAllUnitOfMeasuresQueryHandler(IUnitOfMeasureRepository repo) => _repo = repo;

    public async Task<Result<List<UnitOfMeasure>>> Handle(GetAllUnitOfMeasuresQuery request, CancellationToken ct)
        => Result.Success(await _repo.GetAllAsync(ct));
}
