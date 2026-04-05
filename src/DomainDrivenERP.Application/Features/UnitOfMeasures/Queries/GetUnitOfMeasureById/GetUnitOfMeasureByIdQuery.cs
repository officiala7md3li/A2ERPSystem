using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;

namespace DomainDrivenERP.Application.Features.UnitOfMeasures.Queries.GetUnitOfMeasureById;

public record GetUnitOfMeasureByIdQuery(Guid UomId) : IQuery<UnitOfMeasure>;
