using DomainDrivenERP.Application.Features.Companies.Commands.CreateCompany;
using DomainDrivenERP.Application.Features.Companies.Queries.GetAllCompanies;
using DomainDrivenERP.Application.Features.Companies.Queries.GetCompanyById;
using DomainDrivenERP.Domain.Entities.Companies;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[AllowAnonymous]
[Route("api/v1/companies")]
public class CompaniesController : AppControllerBase
{
    public CompaniesController(ISender sender) : base(sender) { }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCompanyCommand request, CancellationToken ct)
    {
        Result<Company> result = await Sender.Send(request, ct);
        return CustomResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        Result<List<Company>> result = await Sender.Send(new GetAllCompaniesQuery(), ct);
        return CustomResult(result);
    }

    [HttpGet("{companyId:guid}")]
    public async Task<IActionResult> GetById(Guid companyId, CancellationToken ct)
    {
        Result<Company> result = await Sender.Send(new GetCompanyByIdQuery(companyId), ct);
        return CustomResult(result);
    }
}
