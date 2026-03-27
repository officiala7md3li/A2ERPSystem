using DomainDrivenERP.Application.Features.Invoices.Commands.AddLineToInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.CancelInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.CreateCustomerInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.PostInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.SubmitInvoice;
using DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoiceById;
using DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoices;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Presentation.Controllers;

[Route("api/v1/invoices/customer")]
public sealed class CustomerInvoicesController : AppControllerBase
{
    public CustomerInvoicesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerInvoiceCommand request, CancellationToken cancellationToken)
    {
        Result<CreateCustomerInvoiceResult> result = await Sender.Send(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        Result<CustomerInvoiceDetailDto> result = await Sender.Send(new GetCustomerInvoiceByIdQuery(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomerId(Guid customerId, CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<CustomerInvoiceListDto>> result = await Sender.Send(new GetCustomerInvoicesQuery(customerId), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/lines")]
    public async Task<IActionResult> AddLine(Guid id, [FromBody] AddLineToInvoiceRequest request, CancellationToken cancellationToken)
    {
        var command = new AddLineToInvoiceCommand(
            id,
            request.ItemId,
            request.Quantity,
            request.QuantityUnit,
            request.UnitPrice,
            request.Currency,
            request.TaxGroupId,
            request.DiscountGroupId,
            request.SortOrder);

        Result<AddLineToInvoiceResult> result = await Sender.Send(command, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        Result result = await Sender.Send(new SubmitInvoiceCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(Guid id, CancellationToken cancellationToken)
    {
        Result<PostInvoiceResult> result = await Sender.Send(new PostInvoiceCommand(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelInvoiceRequest request, CancellationToken cancellationToken)
    {
        Result result = await Sender.Send(new CancelInvoiceCommand(id, request.Reason), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

public sealed record AddLineToInvoiceRequest(
    Guid ItemId,
    decimal Quantity,
    string QuantityUnit,
    decimal UnitPrice,
    string Currency,
    Guid? TaxGroupId = null,
    Guid? DiscountGroupId = null,
    int SortOrder = 0);

public sealed record CancelInvoiceRequest(string Reason);
