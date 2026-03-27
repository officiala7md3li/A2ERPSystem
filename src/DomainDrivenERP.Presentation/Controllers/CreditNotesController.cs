using DomainDrivenERP.Application.Features.CreditNotes.Commands.AddLineToCreditNote;
using DomainDrivenERP.Application.Features.CreditNotes.Commands.CancelCreditNote;
using DomainDrivenERP.Application.Features.CreditNotes.Commands.CreateCreditNote;
using DomainDrivenERP.Application.Features.CreditNotes.Commands.PostCreditNote;
using DomainDrivenERP.Application.Features.CreditNotes.Commands.SubmitCreditNote;
using DomainDrivenERP.Application.Features.CreditNotes.Queries.GetCreditNoteById;
using DomainDrivenERP.Application.Features.CreditNotes.Queries.GetCreditNotes;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[Route("api/credit-notes")]
public sealed class CreditNotesController : AppControllerBase
{
    public CreditNotesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCreditNoteCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetCreditNoteByIdQuery(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomerId(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetCreditNotesQuery(customerId), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/lines")]
    public async Task<IActionResult> AddLine(
        Guid id,
        [FromBody] AddLineToCreditNoteCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.NoteId)
            return BadRequest("NoteId in the URL does not match the command.");

        var result = await Sender.Send(command, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new SubmitCreditNoteCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new PostCreditNoteCommand(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelCreditNoteCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.NoteId)
            return BadRequest("NoteId in the URL does not match the command.");

        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
