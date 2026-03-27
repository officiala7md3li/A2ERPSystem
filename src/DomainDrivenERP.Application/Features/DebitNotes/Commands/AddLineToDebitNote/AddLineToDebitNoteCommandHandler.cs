using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Invoices;
using DomainDrivenERP.Domain.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.DebitNotes.Commands.AddLineToDebitNote;

internal sealed class AddLineToDebitNoteCommandHandler
    : ICommandHandler<AddLineToDebitNoteCommand, AddLineToDebitNoteResult>
{
    private readonly IDebitNoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddLineToDebitNoteCommandHandler(
        IDebitNoteRepository noteRepository,
        IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddLineToDebitNoteResult>> Handle(
        AddLineToDebitNoteCommand request,
        CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdWithLinesAsync(request.NoteId, cancellationToken);
        if (note is null)
            return Result.Failure<AddLineToDebitNoteResult>(
                new Error("DebitNote.NotFound", $"DebitNote '{request.NoteId}' not found."));

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
            return Result.Failure<AddLineToDebitNoteResult>(lineResult.Error);

        var line = lineResult.Value;

        var addResult = note.AddLine(line);
        if (addResult.IsFailure)
            return Result.Failure<AddLineToDebitNoteResult>(addResult.Error);

        await _noteRepository.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AddLineToDebitNoteResult(
            line.Id,
            line.InvoiceId,
            line.ItemId,
            line.SubTotal,
            line.TotalDiscountAmount,
            line.TotalTaxAmount,
            line.FinalLineTotal));
    }
}
