using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Abstractions.Persistence.Repositories;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Persistence.Clients;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DomainDrivenERP.Persistence.Repositories.Coas;
internal class CoaSqlRepository : ICoaRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly SqlConnection _sqlConnection;

    public CoaSqlRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _sqlConnection = _connectionFactory.SqlConnection();
    }
    public async Task CreateCoa(Accounts cOA, CancellationToken cancellationToken = default)
    {
        await _sqlConnection.ExecuteAsync(
            "INSERT INTO Accounts (Id, HeadCode, HeadName, HeadLevel, ParentAccountId, ParentHeadCode) VALUES (@Id, @HeadCode, @HeadName, @HeadLevel, @ParentAccountId, @ParentHeadCode)",
            cOA);
    }

    public async Task<Accounts?> GetCoaById(Guid coaId, CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.QueryFirstOrDefaultAsync<Accounts>(
            "SELECT * FROM Accounts WHERE Id = @CoaId",
            new { CoaId = coaId });
    }

    public async Task<Accounts?> GetCoaByName(string coaParentName, CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.QueryFirstOrDefaultAsync<Accounts>(
            "SELECT * FROM Accounts WHERE HeadName = @CoaParentName",
            new { CoaParentName = coaParentName });
    }

    public async Task<List<Accounts>?> GetCoaChilds(Guid parentAccountId, CancellationToken cancellationToken = default)
    {
        return (await _sqlConnection.QueryAsync<Accounts>(
            "SELECT * FROM Accounts WHERE ParentAccountId = @ParentAccountId",
            new { ParentAccountId = parentAccountId })).ToList();
    }

    public async Task<Accounts?> GetCoaWithChildren(Guid coaId, CancellationToken cancellationToken = default)
    {
        var coaDictionary = new Dictionary<Guid, Accounts>();
        Accounts? coa = await _sqlConnection.QueryFirstOrDefaultAsync<Accounts>(
            "SELECT * FROM Accounts WHERE Id = @CoaId",
            new { CoaId = coaId });

        if (coa != null)
        {
            await FetchChildCOAs(_sqlConnection, coaDictionary, coa);
        }

        return coa;
    }


    public async Task<bool> IsCoaExist(Guid coaId, CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.ExecuteScalarAsync<bool>(
            "SELECT TOP 1 1 FROM Accounts WHERE Id = @CoaId",
            new { CoaId = coaId });
    }

    public async Task<bool> IsCoaExist(string coaName, string coaParentName, CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.ExecuteScalarAsync<bool>(
            "SELECT TOP 1 1 FROM Accounts c INNER JOIN Accounts p ON c.ParentAccountId = p.Id WHERE c.HeadName = @CoaName AND p.HeadName = @CoaParentName",
            new { CoaName = coaName, CoaParentName = coaParentName });
    }

    public async Task<bool> IsCoaExist(string coaName, int level = 1, CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.ExecuteScalarAsync<bool>(
            "SELECT TOP 1 1 FROM Accounts WHERE HeadName = @CoaName AND HeadLevel = @Level",
            new { CoaName = coaName, Level = level });
    }

    public async Task<string?> GetLastHeadCodeInLevelOne(CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.ExecuteScalarAsync<string>(
            "SELECT MAX(HeadCode) FROM Accounts WHERE HeadLevel = 1");
    }

    public async Task<Guid?> GetByAccountName(string accountName, CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.QueryFirstOrDefaultAsync<Guid?>(
            "SELECT Id FROM Accounts WHERE HeadName = @AccountName",
            new { AccountName = accountName });
    }

    public async Task<Guid?> GetByAccountHeadCode(string accountHeadCode, CancellationToken cancellationToken = default)
    {
        return await _sqlConnection.QueryFirstOrDefaultAsync<Guid?>(
            "SELECT Id FROM Accounts WHERE HeadCode = @AccountHeadCode",
            new { AccountHeadCode = accountHeadCode });
    }

    private async Task FetchChildCOAs(IDbConnection connection, Dictionary<Guid, Accounts> coaDictionary, Accounts coa)
    {
        IEnumerable<Accounts> childCOAs = await connection.QueryAsync<Accounts>(
            "SELECT * FROM Accounts WHERE ParentAccountId = @ParentAccountId",
            new { ParentAccountId = coa.Id });

        foreach (Accounts child in childCOAs)
        {
            if (!coaDictionary.ContainsKey(child.Id))
            {
                coaDictionary[child.Id] = child;
                await FetchChildCOAs(connection, coaDictionary, child);
            }
        }
        coa.InsertChildrens(childCOAs.ToList());
    }
}
