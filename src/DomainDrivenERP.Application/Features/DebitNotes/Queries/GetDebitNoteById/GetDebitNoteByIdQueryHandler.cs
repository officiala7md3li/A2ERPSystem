using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.DebitNotes.Queries.GetDebitNoteById;

internal sealed class GetDebitNoteByIdQueryHandler 
    : IQueryHandler<GetDebitNoteByIdQuery, DebitNoteDetailDto>
{
    private readonly IDebitNoteRepository _noteRepository;

    public GetDebitNoteByIdQueryHandler(IDebitNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<Result<DebitNoteDetailDto>> Handle(
        GetDebitNoteByIdQuery request,
        CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdWithLinesAsync(request.NoteId, cancellationToken);
        
        if (note is null)
            return Result.Failure<DebitNoteDetailDto>(
                new Error("DebitNote.NotFound", $"DebitNote '{request.NoteId}' not found."));

        var dto = new DebitNoteDetailDto(
            note.Id,
            note.CustomerId,
            note.CompanyId,
            note.SequenceNumber,
            note.Reason,
            note.NoteDate,
            note.Status.ToString(),
            note.TotalAmount,
            note.TotalTax,
            note.GrandTotal,
            note.Lines.Select(l => new DebitNoteLineDto(
                l.Id,
                l.ItemId,
                l.Quantity.Value,
                l.Quantity.Unit,
                l.UnitPrice.Amount,
                l.UnitPrice.Currency,
                l.SubTotal,
                l.TotalDiscountAmount,
                l.TotalTaxAmount,
                l.HiddenDiscountAmount,
                l.FinalLineTotal,
                l.TaxBreakdowns.Select(t => new DebitLineTaxBreakdownDto(
                    t.TaxCode, t.TaxName, t.Rate, t.TaxAmount, t.IsWithholding)).ToList(),
                l.DiscountBreakdowns.Select(d => new DebitLineDiscountBreakdownDto(
                    d.Source.ToString(), d.Type.ToString(), d.DiscountValue, d.DiscountAmount)).ToList()
            )).ToList()
        );

        return Result.Success(dto);
    }
}
