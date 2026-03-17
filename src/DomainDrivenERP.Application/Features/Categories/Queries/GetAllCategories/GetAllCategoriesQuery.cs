using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Categories;

namespace DomainDrivenERP.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery() : IListQuery<Category>;
