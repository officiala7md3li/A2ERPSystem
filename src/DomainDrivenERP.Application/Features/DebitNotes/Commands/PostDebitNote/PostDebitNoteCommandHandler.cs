using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.PostDebitNote;

internal sealed class PostDebitNoteCommandHandler 
    : ICommandHandler<PostDebitNoteCommand, PostDebitNoteResult>
{
    private readonly IDebitNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PostDebitNoteCommandHandler(
        IDebitNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PostDebitNoteResult>> Handle(
        PostDebitNoteCommand request,
        CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        if (note is null)
            return Result.Failure<PostDebitNoteResult>(
                new Error("DebitNote.NotFound", $"DebitNote '{request.NoteId}' not found."));

        // TODO: Orchestrator integration (Tax, Discount, Sequence Engine)
        note.GetType().GetProperty("SequenceNumber")?.SetValue(note, $"DN-{System.DateTime.UtcNow:yyyyMMdd}-001");

        var postResult = note.Post($"DN-{System.DateTime.UtcNow:yyyyMMdd}-001", "{}");
        if (postResult.IsFailure)
            return Result.Failure<PostDebitNoteResult>(postResult.Error);

        await _noteRepository.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PostDebitNoteResult(
            note.Id,
            note.SequenceNumber!,
            note.GrandTotal,
            note.Status.ToString()));
    }
}
