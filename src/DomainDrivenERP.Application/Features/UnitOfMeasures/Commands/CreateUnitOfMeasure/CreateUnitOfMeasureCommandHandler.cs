using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.UnitOfMeasures.Commands.CreateUnitOfMeasure;

public class CreateUnitOfMeasureCommandHandler : ICommandHandler<CreateUnitOfMeasureCommand, UnitOfMeasure>
{
    private readonly IUnitOfMeasureRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateUnitOfMeasureCommandHandler(IUnitOfMeasureRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<UnitOfMeasure>> Handle(CreateUnitOfMeasureCommand req, CancellationToken ct)
    {
        var existing = await _repo.GetByCodeAsync(req.Code, ct);
        if (existing is not null)
            return Result.Failure<UnitOfMeasure>("UoM.Duplicate", $"UoM '{req.Code}' already exists.");

        var result = UnitOfMeasure.Create(req.Code, req.NameEn, req.NameAr, req.Type, req.ConversionFactor, req.BaseUomId);
        if (result.IsFailure) return result;

        await _repo.AddAsync(result.Value);
        await _uow.SaveChangesAsync(ct);
        return result;
    }
}
