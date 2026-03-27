using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.PostCreditNote;

internal sealed class PostCreditNoteCommandHandler 
    : ICommandHandler<PostCreditNoteCommand, PostCreditNoteResult>
{
    private readonly ICreditNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PostCreditNoteCommandHandler(
        ICreditNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PostCreditNoteResult>> Handle(
        PostCreditNoteCommand request,
        CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        if (note is null)
            return Result.Failure<PostCreditNoteResult>(
                new Error("CreditNote.NotFound", $"CreditNote '{request.NoteId}' not found."));

        // TODO: Orchestrator integration (Tax, Discount, Sequence Engine)
        note.GetType().GetProperty("SequenceNumber")?.SetValue(note, $"CN-{System.DateTime.UtcNow:yyyyMMdd}-001");

        var postResult = note.Post($"CN-{System.DateTime.UtcNow:yyyyMMdd}-001", "{}");
        if (postResult.IsFailure)
            return Result.Failure<PostCreditNoteResult>(postResult.Error);

        await _noteRepository.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PostCreditNoteResult(
            note.Id,
            note.SequenceNumber!,
            note.GrandTotal,
            note.Status.ToString()));
    }
}
