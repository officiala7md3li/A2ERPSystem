using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Companies.Queries.GetCompanyById;

public class GetCompanyByIdQueryHandler : IQueryHandler<GetCompanyByIdQuery, Company>
{
    private readonly ICompanyRepository _repo;
    public GetCompanyByIdQueryHandler(ICompanyRepository repo) => _repo = repo;

    public async Task<Result<Company>> Handle(GetCompanyByIdQuery request, CancellationToken ct)
    {
        var company = await _repo.GetByIdAsync(request.CompanyId, ct);
        return company is null
            ? Result.Failure<Company>("Company.NotFound", $"Company with ID '{request.CompanyId}' not found.")
            : Result.Success(company);
    }
}
