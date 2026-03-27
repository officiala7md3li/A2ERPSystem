using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.CreateCreditNote;

internal sealed class CreateCreditNoteCommandHandler
    : ICommandHandler<CreateCreditNoteCommand, CreateCreditNoteResult>
{
    private readonly ICreditNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCreditNoteCommandHandler(
        ICreditNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCreditNoteResult>> Handle(
        CreateCreditNoteCommand request,
        CancellationToken cancellationToken)
    {
        var noteResult = CreditNote.Create(
            request.CustomerId,
            request.CompanyId,
            request.CurrencyId,
            request.OriginalInvoiceId,
            request.NoteDate,
            request.Reason);

        if (noteResult.IsFailure)
        {
            return Result.Failure<CreateCreditNoteResult>(noteResult.Error);
        }

        var note = noteResult.Value;

        await _noteRepository.AddAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateCreditNoteResult(
            note.Id,
            note.CustomerId,
            note.NoteDate,
            note.Status.ToString()));
    }
}
