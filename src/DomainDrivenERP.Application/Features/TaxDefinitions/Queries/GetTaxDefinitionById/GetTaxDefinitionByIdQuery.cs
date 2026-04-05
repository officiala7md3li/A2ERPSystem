using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;

namespace DomainDrivenERP.Application.Features.TaxDefinitions.Queries.GetTaxDefinitionById;

public record GetTaxDefinitionByIdQuery(Guid TaxDefinitionId) : IQuery<TaxDefinition>;
