using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Companies;

namespace DomainDrivenERP.Application.Features.Companies.Queries.GetAllCompanies;

public record GetAllCompaniesQuery() : IQuery<List<Company>>;
