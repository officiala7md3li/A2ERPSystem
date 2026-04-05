using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.TaxDefinitions.Commands.CreateTaxDefinition;

public class CreateTaxDefinitionCommandHandler : ICommandHandler<CreateTaxDefinitionCommand, TaxDefinition>
{
    private readonly ITaxDefinitionRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateTaxDefinitionCommandHandler(ITaxDefinitionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<TaxDefinition>> Handle(CreateTaxDefinitionCommand req, CancellationToken ct)
    {
        var existing = await _repo.GetByCodeAsync(req.Code, ct);
        if (existing is not null)
            return Result.Failure<TaxDefinition>("TaxDef.Duplicate", $"Tax definition '{req.Code}' already exists.");

        var result = TaxDefinition.Create(
            req.Code, req.NameEn, req.NameAr, req.CalculationMethod, req.Rate,
            req.IsWithholding, req.AppliesTo);

        if (result.IsFailure) return result;

        await _repo.AddAsync(result.Value);
        await _uow.SaveChangesAsync(ct);
        return result;
    }
}
