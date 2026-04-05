using DomainDrivenERP.Application.Features.UnitOfMeasures.Commands.CreateUnitOfMeasure;
using DomainDrivenERP.Application.Features.UnitOfMeasures.Queries.GetAllUnitOfMeasures;
using DomainDrivenERP.Application.Features.UnitOfMeasures.Queries.GetUnitOfMeasureById;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[AllowAnonymous]
[Route("api/v1/unit-of-measures")]
public class UnitOfMeasuresController : AppControllerBase
{
    public UnitOfMeasuresController(ISender sender) : base(sender) { }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateUnitOfMeasureCommand request, CancellationToken ct)
    {
        Result<UnitOfMeasure> result = await Sender.Send(request, ct);
        return CustomResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        Result<List<UnitOfMeasure>> result = await Sender.Send(new GetAllUnitOfMeasuresQuery(), ct);
        return CustomResult(result);
    }

    [HttpGet("{uomId:guid}")]
    public async Task<IActionResult> GetById(Guid uomId, CancellationToken ct)
    {
        Result<UnitOfMeasure> result = await Sender.Send(new GetUnitOfMeasureByIdQuery(uomId), ct);
        return CustomResult(result);
    }
}
