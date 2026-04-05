using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Companies;

namespace DomainDrivenERP.Application.Features.Companies.Queries.GetCompanyById;

public record GetCompanyByIdQuery(Guid CompanyId) : IQuery<Company>;
