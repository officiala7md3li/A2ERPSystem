using DomainDrivenERP.Application.Features.Customers.Commands.CreateCustomer;
using DomainDrivenERP.Application.Features.Customers.Queries.GetCustomerInvoicesById;
using DomainDrivenERP.Application.Features.Customers.Queries.RetriveCustomer;
using DomainDrivenERP.Application.Features.Customers.Queries.RetriveCustomers;
using DomainDrivenERP.Application.Features.Invoices.Commands.AddLineToInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.CancelInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.CreateCustomerInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.PostInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.SubmitInvoice;
using DomainDrivenERP.Application.Features.Invoices.Queries.RetriveCustomerInvoice;
using DomainDrivenERP.Domain.Entities.Customers;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

/// <summary>Simple DTO for the AddLineToInvoice endpoint — positional records can't bind from JSON.</summary>
public sealed class AddLineRequest
{
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
    public string QuantityUnit { get; set; } = "PCS";
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public Guid? TaxGroupId { get; set; }
    public Guid? DiscountGroupId { get; set; }
    public int SortOrder { get; set; }
}

[Microsoft.AspNetCore.Mvc.Route("api/v1/customers")]
public sealed class CustomersController : AppControllerBase
{
    public CustomersController(ISender sender) : base(sender)
    {

    }    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Result<Customer> result = await Sender.Send(request, cancellationToken);
        return CustomResult(result);
        // return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        Result<RetriveCustomerResponse> result = await Sender.Send(new RetriveCustomerQuery(id), cancellationToken);
        return CustomResult(result);
        // return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] RetriveCustomersQuery request, CancellationToken cancellationToken)
    {
        Result<CustomList<Customer>> result = await Sender.Send(request, cancellationToken);
        return CustomResult(result);
        // return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
    [HttpGet("info/invoices{id}")]
    public async Task<IActionResult> GetCustomerWithInvoices(string id, CancellationToken cancellationToken)
    {
        Result<Customer> result = await Sender.Send(new GetCustomerInvoicesByIdQuery(id), cancellationToken);
        return CustomResult(result);
    }
    [HttpGet("invoices")]
    public async Task<IActionResult> GetCustomerInvoices(
     string customerId,
     DateTime? startDate,
     DateTime? endDate,
     int pageSize = 10,
     int pageNumber = 1,
     CancellationToken cancellationToken = default)
    {
        Result<CustomList<Invoice>> result = await Sender.Send(new RetriveCustomerInvoicesQuery(customerId, startDate, endDate, pageSize, pageNumber), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("invoices/create")]
    public async Task<IActionResult> CreateCustomerInvoice(CreateCustomerInvoiceCommand request, CancellationToken cancellationToken)
    {
        Result<CreateCustomerInvoiceResult> result = await Sender.Send(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("invoices/{invoiceId:guid}/lines")]
    public async Task<IActionResult> AddLineToInvoice(Guid invoiceId, [FromBody] AddLineRequest body, CancellationToken cancellationToken)
    {
        var cmd = new AddLineToInvoiceCommand(
            invoiceId,
            body.ItemId,
            body.Quantity,
            body.QuantityUnit,
            body.UnitPrice,
            body.Currency,
            body.TaxGroupId,
            body.DiscountGroupId,
            body.SortOrder);
        Result<AddLineToInvoiceResult> result = await Sender.Send(cmd, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("invoices/{invoiceId:guid}/submit")]
    public async Task<IActionResult> SubmitInvoice(Guid invoiceId, CancellationToken cancellationToken)
    {
        Result result = await Sender.Send(new SubmitInvoiceCommand(invoiceId), cancellationToken);
        return result.IsSuccess ? Ok(new { succeeded = true }) : BadRequest(new { succeeded = false, message = result.Error.Message });
    }

    [HttpPost("invoices/{invoiceId:guid}/post")]
    public async Task<IActionResult> PostInvoice(Guid invoiceId, CancellationToken cancellationToken)
    {
        Result<PostInvoiceResult> result = await Sender.Send(new PostInvoiceCommand(invoiceId), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("invoices/{invoiceId:guid}/cancel")]
    public async Task<IActionResult> CancelInvoice(Guid invoiceId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        Result result = await Sender.Send(new CancelInvoiceCommand(invoiceId, reason ?? "Cancelled by user"), cancellationToken);
        return result.IsSuccess ? Ok(new { succeeded = true }) : BadRequest(new { succeeded = false, message = result.Error.Message });
    }
}
