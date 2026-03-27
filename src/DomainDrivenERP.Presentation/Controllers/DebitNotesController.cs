using DomainDrivenERP.Application.Features.DebitNotes.Commands.AddLineToDebitNote;
using DomainDrivenERP.Application.Features.DebitNotes.Commands.CancelDebitNote;
using DomainDrivenERP.Application.Features.DebitNotes.Commands.CreateDebitNote;
using DomainDrivenERP.Application.Features.DebitNotes.Commands.PostDebitNote;
using DomainDrivenERP.Application.Features.DebitNotes.Commands.SubmitDebitNote;
using DomainDrivenERP.Application.Features.DebitNotes.Queries.GetDebitNoteById;
using DomainDrivenERP.Application.Features.DebitNotes.Queries.GetDebitNotes;
using DomainDrivenERP.Presentation.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Presentation.Controllers;

[Route("api/debit-notes")]
public sealed class DebitNotesController : AppControllerBase
{
    public DebitNotesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDebitNoteCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetDebitNoteByIdQuery(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomerId(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetDebitNotesQuery(customerId), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/lines")]
    public async Task<IActionResult> AddLine(
        Guid id,
        [FromBody] AddLineToDebitNoteCommand command,
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
        var result = await Sender.Send(new SubmitDebitNoteCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new PostDebitNoteCommand(id), cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelDebitNoteCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.NoteId)
            return BadRequest("NoteId in the URL does not match the command.");

        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
