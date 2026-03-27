using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.SubmitCreditNote;

internal sealed class SubmitCreditNoteCommandHandler : ICommandHandler<SubmitCreditNoteCommand>
{
    private readonly ICreditNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitCreditNoteCommandHandler(
        ICreditNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitCreditNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        if (note is null)
            return Result.Failure(new Error("CreditNote.NotFound", $"CreditNote '{request.NoteId}' not found."));

        var result = note.Submit();
        if (result.IsFailure)
            return result;

        await _noteRepository.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
