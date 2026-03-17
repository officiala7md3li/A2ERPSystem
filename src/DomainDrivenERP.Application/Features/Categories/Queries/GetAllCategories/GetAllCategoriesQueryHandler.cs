using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.Categories;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IListQueryHandler<GetAllCategoriesQuery, Category>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CustomList<Category>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetAllCategoriesAsync(cancellationToken);
    }
}
