using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.TaxDefinitions.Queries.GetAllTaxDefinitions;

public class GetAllTaxDefinitionsQueryHandler : IQueryHandler<GetAllTaxDefinitionsQuery, List<TaxDefinition>>
{
    private readonly ITaxDefinitionRepository _repo;
    public GetAllTaxDefinitionsQueryHandler(ITaxDefinitionRepository repo) => _repo = repo;

    public async Task<Result<List<TaxDefinition>>> Handle(GetAllTaxDefinitionsQuery request, CancellationToken ct)
        => Result.Success(await _repo.GetAllActiveAsync(ct));
}
