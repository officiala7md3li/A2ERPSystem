using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.CreditNotes.Commands.AddLineToCreditNote;

internal sealed class AddLineToCreditNoteCommandHandler
    : ICommandHandler<AddLineToCreditNoteCommand, AddLineToCreditNoteResult>
{
    private readonly ICreditNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddLineToCreditNoteCommandHandler(
        ICreditNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddLineToCreditNoteResult>> Handle(
        AddLineToCreditNoteCommand request,
        CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdWithLinesAsync(request.NoteId, cancellationToken);
        if (note is null)
            return Result.Failure<AddLineToCreditNoteResult>(
                new Error("CreditNote.NotFound", $"CreditNote '{request.NoteId}' not found."));

        var lineResult = InvoiceLine.Create(
            request.NoteId,
            request.ItemId,
            request.Quantity,
            request.QuantityUnit,
            request.UnitPrice,
            request.Currency,
            request.TaxGroupId,
            request.DiscountGroupId,
            request.SortOrder);

        if (lineResult.IsFailure)
            return Result.Failure<AddLineToCreditNoteResult>(lineResult.Error);

        var line = lineResult.Value;

        var addResult = note.AddLine(line);
        if (addResult.IsFailure)
            return Result.Failure<AddLineToCreditNoteResult>(addResult.Error);

        await _noteRepository.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AddLineToCreditNoteResult(
            line.Id,
            line.InvoiceId,
            line.ItemId,
            line.SubTotal,
            line.TotalDiscountAmount,
            line.TotalTaxAmount,
            line.FinalLineTotal));
    }
}
