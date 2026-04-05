using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Currencies.Commands.CreateCurrency;

public class CreateCurrencyCommandHandler : ICommandHandler<CreateCurrencyCommand, Currency>
{
    private readonly ICurrencyRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateCurrencyCommandHandler(ICurrencyRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Currency>> Handle(CreateCurrencyCommand request, CancellationToken ct)
    {
        var existing = await _repo.GetByCodeAsync(request.Code, ct);
        if (existing is not null)
            return Result.Failure<Currency>("Currency.Duplicate", $"Currency '{request.Code}' already exists.");

        var result = Currency.Create(request.Code, request.NameEn, request.NameAr, request.Symbol, request.IsBase);
        if (result.IsFailure) return result;

        await _repo.AddAsync(result.Value);
        await _uow.SaveChangesAsync(ct);
        return result;
    }
}
