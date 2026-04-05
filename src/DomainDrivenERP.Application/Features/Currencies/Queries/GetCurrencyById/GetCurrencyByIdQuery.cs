using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Currencies;

namespace DomainDrivenERP.Application.Features.Currencies.Queries.GetCurrencyById;

public record GetCurrencyByIdQuery(Guid CurrencyId) : IQuery<Currency>;
