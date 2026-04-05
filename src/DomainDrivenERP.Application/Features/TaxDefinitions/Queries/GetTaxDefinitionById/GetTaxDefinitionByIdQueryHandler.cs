using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.TaxDefinitions.Queries.GetTaxDefinitionById;

public class GetTaxDefinitionByIdQueryHandler : IQueryHandler<GetTaxDefinitionByIdQuery, TaxDefinition>
{
    private readonly ITaxDefinitionRepository _repo;
    public GetTaxDefinitionByIdQueryHandler(ITaxDefinitionRepository repo) => _repo = repo;

    public async Task<Result<TaxDefinition>> Handle(GetTaxDefinitionByIdQuery request, CancellationToken ct)
    {
        var tax = await _repo.GetWithDependenciesAsync(request.TaxDefinitionId, ct);
        return tax is null
            ? Result.Failure<TaxDefinition>("TaxDef.NotFound", $"Tax definition '{request.TaxDefinitionId}' not found.")
            : Result.Success(tax);
    }
}
