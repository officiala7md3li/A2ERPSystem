using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Shared.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenERP.Application.Features.DebitNotes.Queries.GetDebitNotes;

internal sealed class GetDebitNotesQueryHandler 
    : IQueryHandler<GetDebitNotesQuery, IReadOnlyList<DebitNoteListDto>>
{
    private readonly IDebitNoteRepository _noteRepository;

    public GetDebitNotesQueryHandler(IDebitNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<Result<IReadOnlyList<DebitNoteListDto>>> Handle(
        GetDebitNotesQuery request,
        CancellationToken cancellationToken)
    {
        var notes = await _noteRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        
        var dtos = notes.Select(i => new DebitNoteListDto(
            i.Id,
            i.CustomerId,
            i.SequenceNumber,
            i.Reason,
            i.NoteDate,
            i.Status.ToString(),
            i.GrandTotal
        )).ToList();

        return Result.Success<IReadOnlyList<DebitNoteListDto>>(dtos);
    }
}
