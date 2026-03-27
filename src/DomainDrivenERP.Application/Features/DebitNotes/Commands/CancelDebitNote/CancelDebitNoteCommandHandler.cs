using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.CancelDebitNote;

internal sealed class CancelDebitNoteCommandHandler : ICommandHandler<CancelDebitNoteCommand>
{
    private readonly IDebitNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelDebitNoteCommandHandler(
        IDebitNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelDebitNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        if (note is null)
            return Result.Failure(new Error("DebitNote.NotFound", $"DebitNote '{request.NoteId}' not found."));

        var cancelResult = note.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult;

        await _noteRepository.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
