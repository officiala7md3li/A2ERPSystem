using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.UnitOfMeasures;

namespace DomainDrivenERP.Application.Features.UnitOfMeasures.Queries.GetAllUnitOfMeasures;

public record GetAllUnitOfMeasuresQuery() : IQuery<List<UnitOfMeasure>>;
