using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using DomainDrivenERP.Domain.Abstractions.Persistence.Data;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Entities.COAs.Specifications;
using DomainDrivenERP.Domain.Shared.Specifications;
using Newtonsoft.Json.Serialization;

namespace DomainDrivenERP.Persistence.Repositories.Coas;
internal class CoaSpecificationRepository : ICoaRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public CoaSpecificationRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateCoa(Accounts cOA, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Repository<Accounts>().AddAsync(cOA, cancellationToken);
    }

    public async Task<string?> GetByAccountHeadCode(string accountHeadCode, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = GetCOAByHeadCodeSpecification.GetCOAByHeadCodeSpec(accountHeadCode);
        Accounts? result = await _unitOfWork.Repository<Accounts>().FirstOrDefaultAsync(spec, false, cancellationToken);
        return result?.HeadCode;
    }

    public async Task<string?> GetByAccountName(string accountName, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = GetCOAByAccountNameSpecification.GetCOAByAccountNameSpec(accountName);
        Accounts? result = await _unitOfWork.Repository<Accounts>().FirstOrDefaultAsync(spec, false, cancellationToken);
        return result?.HeadCode;
    }

    public async Task<Accounts?> GetCoaById(string coaId, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = GetCOAByHeadCodeSpecification.GetCOAByHeadCodeSpec(coaId);
        return await _unitOfWork.Repository<Accounts>().FirstOrDefaultAsync(spec, false, cancellationToken);
    }

    public async Task<Accounts?> GetCoaByName(string coaParentName, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = GetCOAByHeadNameSpecification.GetCOAByHeadNameSpec(coaParentName);
        return await _unitOfWork.Repository<Accounts>().FirstOrDefaultAsync(spec, false, cancellationToken);
    }

    public async Task<List<Accounts>?> GetCoaChilds(string parentCoaId, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = GetCOAChildsSpecification.GetCOAChildsSpec(parentCoaId);
        return (List<Accounts>?)await _unitOfWork.Repository<Accounts>().ListAsync(spec, false, cancellationToken);
    }

    public async Task<Accounts?> GetCoaWithChildren(string coaId, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = GetCOAWithChildrenSpecification.GetCOAWithChildrenSpec(coaId);
        return await _unitOfWork.Repository<Accounts>().FirstOrDefaultAsync(spec, false, cancellationToken);
    }

    public async Task<string?> GetLastHeadCodeInLevelOne(CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = GetLastHeadCodeInLevelOneSpecification.GetLastHeadCodeInLevelOneSpec();
        Accounts? result = await _unitOfWork.Repository<Accounts>().FirstOrDefaultAsync(spec, false, cancellationToken);
        return result?.HeadCode;
    }

    public async Task<bool> IsCoaExist(string coaId, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = IsCoaExistByIdSpecification.IsCoaExistByIdSpec(coaId);
        return await _unitOfWork.Repository<Accounts>().AnyAsync(spec, false, cancellationToken);
    }

    public async Task<bool> IsCoaExist(string coaName, int level = 1, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = IsCoaExistByNameAndLevelSpecification.IsCoaExistByNameAndLevelSpec(coaName, level);
        return await _unitOfWork.Repository<Accounts>().AnyAsync(spec, false, cancellationToken);
    }

    public async Task<bool> IsCoaExist(string coaName, string coaParentName, CancellationToken cancellationToken = default)
    {
        BaseSpecification<Accounts> spec = IsCoaExistByNameAndParentNameSpecification.IsCoaExistByNameAndParentNameSpec(coaName, coaParentName);
        return await _unitOfWork.Repository<Accounts>().AnyAsync(spec, false, cancellationToken);
    }
}
