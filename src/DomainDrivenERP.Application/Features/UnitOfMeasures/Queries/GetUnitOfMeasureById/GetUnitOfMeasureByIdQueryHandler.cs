using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.UnitOfMeasures.Queries.GetUnitOfMeasureById;

public class GetUnitOfMeasureByIdQueryHandler : IQueryHandler<GetUnitOfMeasureByIdQuery, UnitOfMeasure>
{
    private readonly IUnitOfMeasureRepository _repo;
    public GetUnitOfMeasureByIdQueryHandler(IUnitOfMeasureRepository repo) => _repo = repo;

    public async Task<Result<UnitOfMeasure>> Handle(GetUnitOfMeasureByIdQuery request, CancellationToken ct)
    {
        var uom = await _repo.GetByIdAsync(request.UomId, ct);
        return uom is null
            ? Result.Failure<UnitOfMeasure>("UoM.NotFound", $"UoM with ID '{request.UomId}' not found.")
            : Result.Success(uom);
    }
}
