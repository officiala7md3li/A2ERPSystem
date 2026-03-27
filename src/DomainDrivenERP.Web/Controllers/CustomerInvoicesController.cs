using DomainDrivenERP.Application.Features.Invoices.Commands.AddLineToInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.CancelInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.CreateCustomerInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.PostInvoice;
using DomainDrivenERP.Application.Features.Invoices.Commands.SubmitInvoice;
using DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoiceById;
using DomainDrivenERP.Application.Features.Invoices.Queries.GetCustomerInvoices;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[Authorize]
[Route("api/invoices/customer")]
public sealed class CustomerInvoicesController : AppControllerBase
{
    public CustomerInvoicesController(ISender sender) : base(sender) { }

    /// <summary>إنشاء فاتورة عميل جديدة (Draft)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCustomerInvoiceResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerInvoiceCommand command,
        CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.InvoiceId }, result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>جلب فاتورة بالـ ID مع كل التفاصيل</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerInvoiceDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new GetCustomerInvoiceByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>جلب كل فواتير عميل معين</summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerInvoiceSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken ct)
    {
        var result = await Sender.Send(new GetCustomerInvoicesQuery(customerId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>إضافة Line للفاتورة</summary>
    [HttpPost("{id:guid}/lines")]
    [ProducesResponseType(typeof(AddLineToInvoiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddLine(
        Guid id,
        [FromBody] AddLineToInvoiceRequest request,
        CancellationToken ct)
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

        var result = await Sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>تحويل الفاتورة من Draft لـ Pending</summary>
    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new SubmitInvoiceCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>ترحيل الفاتورة — تشغيل الـ Orchestrator الكامل</summary>
    [HttpPost("{id:guid}/post")]
    [ProducesResponseType(typeof(PostInvoiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new PostInvoiceCommand(id), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>إلغاء الفاتورة</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelInvoiceRequest request,
        CancellationToken ct)
    {
        var result = await Sender.Send(new CancelInvoiceCommand(id, request.Reason), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}

// ── Request DTOs (Input Models) ───────────────────────────────────────────────

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
