using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DomainDrivenERP.Application.Engines.SequenceEngine;
using DomainDrivenERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenERP.Persistence.Repositories.Sequences;

public sealed class SequenceStore : ISequenceStore
{
    private readonly ApplicationDbContext _context;

    public SequenceStore(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> GetCurrentCounterAsync(
        string prefix, Guid companyId, DateTime date, CancellationToken ct = default)
    {
        var conn = _context.Database.GetDbConnection();

        var sql = @"
            SELECT ISNULL(CounterValue, 0)
            FROM SequenceCounters
            WHERE Prefix = @Prefix AND CompanyId = @CompanyId AND SequenceDate = @Date";

        var current = await conn.ExecuteScalarAsync<long?>(
            sql,
            new { Prefix = prefix, CompanyId = companyId, Date = date.Date });

        return current ?? 0;
    }

    public async Task IncrementAsync(
        string prefix, Guid companyId, DateTime date, long counterValue, CancellationToken ct = default)
    {
        var conn = _context.Database.GetDbConnection();

        var sql = @"
            MERGE SequenceCounters WITH (HOLDLOCK) AS target
            USING (VALUES (@Prefix, @CompanyId, @Date, @Counter))
                AS source (Prefix, CompanyId, SequenceDate, CounterValue)
            ON  target.Prefix = source.Prefix
            AND target.CompanyId = source.CompanyId
            AND target.SequenceDate = source.SequenceDate
            WHEN MATCHED AND source.CounterValue > target.CounterValue THEN
                UPDATE SET CounterValue = source.CounterValue
            WHEN NOT MATCHED THEN
                INSERT (Id, Prefix, CompanyId, SequenceDate, CounterValue)
                VALUES (NEWID(), source.Prefix, source.CompanyId, source.SequenceDate, source.CounterValue);";

        await conn.ExecuteAsync(sql, new { Prefix = prefix, CompanyId = companyId, Date = date.Date, Counter = counterValue });
    }

    public async Task<long> IncrementWithLockAsync(
        string prefix, Guid companyId, DateTime date, CancellationToken ct = default)
    {
        var effectiveDate = date.Date;

        // Use EF's ExecuteSqlRawAsync to avoid Dapper's separate transaction conflicting with EF's connection state.
        var upsertSql = @"
            MERGE SequenceCounters WITH (HOLDLOCK) AS target
            USING (VALUES ({0}, {1}, {2}, CAST(1 AS BIGINT)))
                AS source (Prefix, CompanyId, SequenceDate, CounterValue)
            ON  target.Prefix = source.Prefix
            AND target.CompanyId = source.CompanyId
            AND target.SequenceDate = source.SequenceDate
            WHEN MATCHED THEN
                UPDATE SET CounterValue = target.CounterValue + 1
            WHEN NOT MATCHED THEN
                INSERT (Id, Prefix, CompanyId, SequenceDate, CounterValue)
                VALUES (NEWID(), source.Prefix, source.CompanyId, source.SequenceDate, source.CounterValue);";

        await _context.Database.ExecuteSqlRawAsync(upsertSql, new object[] { prefix, companyId, effectiveDate }, ct);

        // Read back the counter value
        var conn = _context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync(ct);

        var nextValue = await conn.ExecuteScalarAsync<long>(
            "SELECT CounterValue FROM SequenceCounters WHERE Prefix = @Prefix AND CompanyId = @CompanyId AND SequenceDate = @Date",
            new { Prefix = prefix, CompanyId = companyId, Date = effectiveDate });

        return nextValue;
    }
}
