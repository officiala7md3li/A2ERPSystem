using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Persistence.Clients;
using DomainDrivenERP.Persistence.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.Coa;
internal sealed class CoaRepository : ICoaRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ISqlConnectionFactory _connectionFactory;
    public CoaRepository(ApplicationDbContext context, ISqlConnectionFactory connectionFactory)
    {
        _context = context;
        _connectionFactory = connectionFactory;
    }

    public async Task CreateCoa(Accounts cOA, CancellationToken cancellationToken = default)
    {
        await _context.Set<Accounts>().AddAsync(cOA);
    }

    public async Task<Accounts?> GetCoaById(string coaId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>().FirstOrDefaultAsync(a => a.HeadCode == coaId);
    }

    public async Task<Accounts?> GetCoaByName(string coaParentName, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>().Include(a => a.ChildAccounts).FirstOrDefaultAsync(a => a.HeadName == coaParentName);
    }

    public async Task<List<Accounts>?> GetCoaChilds(string parentCoaId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>().Where(a => a.ParentHeadCode == parentCoaId).ToListAsync();
    }

    public async Task<Accounts?> GetCoaWithChildren(string coaId, CancellationToken cancellationToken = default)
    {
        Accounts? coa = await _context.Set<Accounts>()
            .FirstOrDefaultAsync(c => c.HeadCode == coaId);

        if (coa != null)
        {
            await LoadChildrenRecursively(coa);
        }

        return coa;
    }
    public async Task<bool> IsCoaExist(string coaId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>().AnyAsync(coa => coa.HeadCode == coaId, cancellationToken);
    }

    public async Task<bool> IsCoaExist(string coaName, string coaParentName, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>().Include(a => a.ParentAccount)
            .AnyAsync(coa => coa.HeadName == coaName && coa.ParentAccount.HeadName == coaParentName);
    }

    public async Task<bool> IsCoaExist(string coaName, int level = 1, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>()
             .AnyAsync(coa => coa.HeadName == coaName && coa.HeadLevel == level);
    }

    public async Task<string?> GetLastHeadCodeInLevelOne(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>().Where(a => a.HeadLevel == 1).MaxAsync(coa => coa.HeadCode);
    }

    public async Task<string?> GetByAccountName(string accountName, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>()
                             .Where(coa => coa.HeadName == accountName)
                             .Select(coa => coa.HeadCode)
                             .FirstOrDefaultAsync();
    }

    public async Task<string?> GetByAccountHeadCode(string accountHeadCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Accounts>()
                             .Where(coa => coa.HeadCode == accountHeadCode)
                             .Select(coa => coa.HeadCode)
                             .FirstOrDefaultAsync();
    }


    private async Task LoadChildrenRecursively(Accounts coa)
    {
        await _context.Entry(coa)
            .Collection(c => c.ChildAccounts)
            .LoadAsync();

        foreach (Accounts? child in coa.ChildAccounts.ToList())
        {
            await LoadChildrenRecursively(child);
        }
    }
}
