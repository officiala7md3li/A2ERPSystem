using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Companies.Queries.GetAllCompanies;

public class GetAllCompaniesQueryHandler : IQueryHandler<GetAllCompaniesQuery, List<Company>>
{
    private readonly ICompanyRepository _repo;
    public GetAllCompaniesQueryHandler(ICompanyRepository repo) => _repo = repo;

    public async Task<Result<List<Company>>> Handle(GetAllCompaniesQuery request, CancellationToken ct)
        => Result.Success(await _repo.GetAllAsync(ct));
}
