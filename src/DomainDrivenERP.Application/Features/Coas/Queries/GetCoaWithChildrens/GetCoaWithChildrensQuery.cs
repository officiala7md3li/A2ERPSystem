using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Dtos;

namespace DomainDrivenERP.Application.Features.Coas.Queries.GetCoaWithChildrens;
public class GetCoaWithChildrensQuery : IQuery<CoaWithChildrenDto>
{
    public GetCoaWithChildrensQuery(Guid coaId)
    {
        CoaId = coaId;
    }

    public Guid CoaId { get; }
}
