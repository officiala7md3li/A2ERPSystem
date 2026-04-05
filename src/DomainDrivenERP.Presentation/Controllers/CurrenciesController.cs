using DomainDrivenERP.Application.Features.Currencies.Commands.CreateCurrency;
using DomainDrivenERP.Application.Features.Currencies.Queries.GetAllCurrencies;
using DomainDrivenERP.Application.Features.Currencies.Queries.GetCurrencyById;
using DomainDrivenERP.Domain.Entities.Currencies;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[AllowAnonymous]
[Route("api/v1/currencies")]
public class CurrenciesController : AppControllerBase
{
    public CurrenciesController(ISender sender) : base(sender) { }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCurrencyCommand request, CancellationToken ct)
    {
        Result<Currency> result = await Sender.Send(request, ct);
        return CustomResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        Result<List<Currency>> result = await Sender.Send(new GetAllCurrenciesQuery(), ct);
        return CustomResult(result);
    }

    [HttpGet("{currencyId:guid}")]
    public async Task<IActionResult> GetById(Guid currencyId, CancellationToken ct)
    {
        Result<Currency> result = await Sender.Send(new GetCurrencyByIdQuery(currencyId), ct);
        return CustomResult(result);
    }
}
