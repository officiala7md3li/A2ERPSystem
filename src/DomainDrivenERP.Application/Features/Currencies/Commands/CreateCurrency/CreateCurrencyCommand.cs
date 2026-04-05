using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Currencies;

namespace DomainDrivenERP.Application.Features.Currencies.Commands.CreateCurrency;

public record CreateCurrencyCommand(
    string Code,
    string NameEn,
    string NameAr,
    string Symbol,
    bool IsBase = false) : ICommand<Currency>;
