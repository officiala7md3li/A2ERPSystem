using DomainDrivenERP.Application.Features.VendorInvoices.Commands.AddLineToVendorInvoice;
using DomainDrivenERP.Application.Features.VendorInvoices.Commands.CancelVendorInvoice;
using DomainDrivenERP.Application.Features.VendorInvoices.Commands.CreateVendorInvoice;
using DomainDrivenERP.Application.Features.VendorInvoices.Commands.PostVendorInvoice;
using DomainDrivenERP.Application.Features.VendorInvoices.Commands.SubmitVendorInvoice;
using DomainDrivenERP.Application.Features.VendorInvoices.Queries.GetVendorInvoiceById;
using DomainDrivenERP.Application.Features.VendorInvoices.Queries.GetVendorInvoices;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[Route("api/vendor-invoices")]
public sealed class VendorInvoicesController : AppControllerBase
{
    public VendorInvoicesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVendorInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetVendorInvoiceByIdQuery(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("vendor/{vendorId:guid}")]
    public async Task<IActionResult> GetByVendorId(Guid vendorId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetVendorInvoicesQuery(vendorId), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/lines")]
    public async Task<IActionResult> AddLine(
        Guid id,
        [FromBody] AddLineToVendorInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.InvoiceId)
            return BadRequest("InvoiceId in the URL does not match the command.");

        var result = await Sender.Send(command, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new SubmitVendorInvoiceCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new PostVendorInvoiceCommand(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelVendorInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.InvoiceId)
            return BadRequest("InvoiceId in the URL does not match the command.");

        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
