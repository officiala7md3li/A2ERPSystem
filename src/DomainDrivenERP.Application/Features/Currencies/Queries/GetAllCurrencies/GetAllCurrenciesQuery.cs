using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Currencies;

namespace DomainDrivenERP.Application.Features.Currencies.Queries.GetAllCurrencies;

public record GetAllCurrenciesQuery() : IQuery<List<Currency>>;
