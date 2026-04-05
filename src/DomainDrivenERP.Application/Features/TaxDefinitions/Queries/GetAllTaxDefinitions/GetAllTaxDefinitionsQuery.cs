using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.TaxDefinitions;

namespace DomainDrivenERP.Application.Features.TaxDefinitions.Queries.GetAllTaxDefinitions;

public record GetAllTaxDefinitionsQuery() : IQuery<List<TaxDefinition>>;
