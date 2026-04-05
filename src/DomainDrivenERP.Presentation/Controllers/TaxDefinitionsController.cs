using DomainDrivenERP.Application.Features.TaxDefinitions.Commands.CreateTaxDefinition;
using DomainDrivenERP.Application.Features.TaxDefinitions.Queries.GetAllTaxDefinitions;
using DomainDrivenERP.Application.Features.TaxDefinitions.Queries.GetTaxDefinitionById;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[AllowAnonymous]
[Route("api/v1/tax-definitions")]
public class TaxDefinitionsController : AppControllerBase
{
    public TaxDefinitionsController(ISender sender) : base(sender) { }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateTaxDefinitionCommand request, CancellationToken ct)
    {
        Result<TaxDefinition> result = await Sender.Send(request, ct);
        return CustomResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        Result<List<TaxDefinition>> result = await Sender.Send(new GetAllTaxDefinitionsQuery(), ct);
        return CustomResult(result);
    }

    [HttpGet("{taxDefId:guid}")]
    public async Task<IActionResult> GetById(Guid taxDefId, CancellationToken ct)
    {
        Result<TaxDefinition> result = await Sender.Send(new GetTaxDefinitionByIdQuery(taxDefId), ct);
        return CustomResult(result);
    }
}
