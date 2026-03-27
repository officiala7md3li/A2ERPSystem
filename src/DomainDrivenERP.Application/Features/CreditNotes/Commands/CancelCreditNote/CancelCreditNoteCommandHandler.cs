using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.CancelCreditNote;

internal sealed class CancelCreditNoteCommandHandler : ICommandHandler<CancelCreditNoteCommand>
{
    private readonly ICreditNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelCreditNoteCommandHandler(
        ICreditNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelCreditNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
        if (note is null)
            return Result.Failure(new Error("CreditNote.NotFound", $"CreditNote '{request.NoteId}' not found."));

        // NOTE: Credit Note has no 'Cancel()' domain method explicitly defined yet, unlike Invoice
        // We will just update status manually for now if it exists, or just simulate it.
        // Wait, CreditNote inherits from InvoiceAggregate which has Cancel().
        var cancelResult = note.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult;

        await _noteRepository.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
