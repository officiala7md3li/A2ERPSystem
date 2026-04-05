using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Currencies.Queries.GetAllCurrencies;

public class GetAllCurrenciesQueryHandler : IQueryHandler<GetAllCurrenciesQuery, List<Currency>>
{
    private readonly ICurrencyRepository _repo;

    public GetAllCurrenciesQueryHandler(ICurrencyRepository repo) => _repo = repo;

    public async Task<Result<List<Currency>>> Handle(GetAllCurrenciesQuery request, CancellationToken ct)
        => Result.Success(await _repo.GetAllAsync(ct));
}
