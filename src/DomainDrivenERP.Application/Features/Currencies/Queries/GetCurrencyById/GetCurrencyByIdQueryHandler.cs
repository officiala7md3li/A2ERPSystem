using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Currencies.Queries.GetCurrencyById;

public class GetCurrencyByIdQueryHandler : IQueryHandler<GetCurrencyByIdQuery, Currency>
{
    private readonly ICurrencyRepository _repo;

    public GetCurrencyByIdQueryHandler(ICurrencyRepository repo) => _repo = repo;

    public async Task<Result<Currency>> Handle(GetCurrencyByIdQuery request, CancellationToken ct)
    {
        var currency = await _repo.GetByIdAsync(request.CurrencyId, ct);
        return currency is null
            ? Result.Failure<Currency>("Currency.NotFound", $"Currency with ID '{request.CurrencyId}' not found.")
            : Result.Success(currency);
    }
}
