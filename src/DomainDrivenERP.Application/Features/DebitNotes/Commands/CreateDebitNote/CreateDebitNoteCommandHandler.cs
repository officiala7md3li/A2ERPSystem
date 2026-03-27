using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.CreateDebitNote;

internal sealed class CreateDebitNoteCommandHandler
    : ICommandHandler<CreateDebitNoteCommand, CreateDebitNoteResult>
{
    private readonly IDebitNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDebitNoteCommandHandler(
        IDebitNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateDebitNoteResult>> Handle(
        CreateDebitNoteCommand request,
        CancellationToken cancellationToken)
    {
        var noteResult = DebitNote.Create(
            request.CustomerId,
            request.CompanyId,
            request.CurrencyId,
            request.OriginalInvoiceId,
            request.NoteDate,
            request.Reason);

        if (noteResult.IsFailure)
        {
            return Result.Failure<CreateDebitNoteResult>(noteResult.Error);
        }

        var note = noteResult.Value;

        await _noteRepository.AddAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateDebitNoteResult(
            note.Id,
            note.CustomerId,
            note.NoteDate,
            note.Status.ToString()));
    }
}
