using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.CreditNotes.Queries.GetCreditNotes;

internal sealed class GetCreditNotesQueryHandler 
    : IQueryHandler<GetCreditNotesQuery, IReadOnlyList<CreditNoteListDto>>
{
    private readonly ICreditNoteRepository _noteRepository;

    public GetCreditNotesQueryHandler(ICreditNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<Result<IReadOnlyList<CreditNoteListDto>>> Handle(
        GetCreditNotesQuery request,
        CancellationToken cancellationToken)
    {
        var notes = await _noteRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        
        var dtos = notes.Select(i => new CreditNoteListDto(
            i.Id,
            i.CustomerId,
            i.SequenceNumber,
            i.Reason,
            i.NoteDate,
            i.Status.ToString(),
            i.GrandTotal
        )).ToList();

        return Result.Success<IReadOnlyList<CreditNoteListDto>>(dtos);
    }
}
