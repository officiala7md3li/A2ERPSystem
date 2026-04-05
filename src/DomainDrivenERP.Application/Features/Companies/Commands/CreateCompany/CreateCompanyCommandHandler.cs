using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand, Company>
{
    private readonly ICompanyRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateCompanyCommandHandler(ICompanyRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Company>> Handle(CreateCompanyCommand req, CancellationToken ct)
    {
        var result = Company.Create(
            req.NameEn, req.NameAr, req.TaxRegistrationNumber, req.BaseCurrencyId,
            req.DefaultTaxOrder, req.DefaultStackingMode, req.StockValuation);

        if (result.IsFailure) return result;

        await _repo.AddAsync(result.Value);
        await _uow.SaveChangesAsync(ct);
        return result;
    }
}
