using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DomainDrivenERP.Domain.Entities.COAs.DomainEvents;
using DomainDrivenERP.Domain.Entities.Transactions;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Guards;
using DomainDrivenERP.Domain.Shared.Results;

namespace DomainDrivenERP.Domain.Entities.COAs;

public sealed class Accounts : AggregateRoot
{
    private readonly List<Accounts> _accounts = new();
    private readonly List<Transaction> _transactions = new();
    public Accounts()
    {

    }
    public Accounts(Guid id, string headCode, string headName, Guid? parentAccountId, string parentHeadCode, bool isGl, ChartOfAccountsType type, int headLevel)
        : base(id)
    {
        Guard.Against.NullOrEmpty(headCode, nameof(headCode));
        Guard.Against.NullOrEmpty(headName, nameof(headName));

        HeadCode = headCode;
        HeadName = headName;
        ParentAccountId = parentAccountId;
        ParentHeadCode = parentHeadCode;
        IsGl = isGl;
        Type = type;
        HeadLevel = headLevel;
    }
    public Accounts(Guid id, string headCode, string headName, bool isGl, ChartOfAccountsType type)
        : base(id)
    {
        Guard.Against.NullOrEmpty(headCode, nameof(headCode));
        Guard.Against.NullOrEmpty(headName, nameof(headName));
        HeadCode = headCode;
        HeadName = headName;
        IsGl = isGl;
        Type = type;
        HeadLevel = 1;
    }
    public string HeadCode { get; private set; }
    public string HeadName { get; private set; }
    public Guid? ParentAccountId { get; private set; }
    public string ParentHeadCode { get; private set; }
    public int HeadLevel { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsGl { get; private set; }
    public ChartOfAccountsType Type { get; private set; }
    public IReadOnlyCollection<Accounts> ChildAccounts => _accounts;
    public IReadOnlyCollection<Transaction> Transactions => _transactions;
    public Accounts ParentAccount { get; private set; }

    public static Result<Accounts> Create(string headName, Accounts? parentCoa, bool isGl = false)
    {
        Guard.Against.NullOrEmpty(headName, nameof(headName));
        Guard.Against.Null(parentCoa, nameof(parentCoa));

        if (parentCoa == null)
        {
            return Result.Failure<Accounts>(new Error("Accounts.Create", "Parent Accounts cannot be null."));
        }

        try
        {
            int headLevel = parentCoa.HeadLevel + 1;
            string headCode = GenerateNextHeadCode(parentCoa);
            Guid id = Guid.NewGuid();

            var coa = new Accounts(id, headCode, headName, parentCoa.Id, parentCoa.HeadCode, isGl, parentCoa.Type, headLevel);
            parentCoa._accounts.Add(coa);
            coa.RaiseDomainEvent(new CreateCOADomainEvent(headName, parentCoa.Id, parentCoa.HeadCode, parentCoa.Type));

            return coa;
        }
        catch (Exception ex)
        {
            return Result.Failure<Accounts>(new Error("Accounts.Create", $"Failed to create Accounts: {ex.Message}"));
        }
    }
    public static Result<Accounts> Create(string headName, string headCode, ChartOfAccountsType type, bool isGl = false)
    {
        Guard.Against.NullOrEmpty(headName, nameof(headName));
        Guard.Against.NullOrEmpty(headCode, nameof(headCode));

        if (string.IsNullOrEmpty(headCode))
        {
            return Result.Failure<Accounts>(new Error("Accounts.Create", "Head code cannot be null or empty."));
        }

        try
        {
            Guid id = Guid.NewGuid();
            var coa = new Accounts(id, headCode, headName, isGl, type);
            coa.RaiseDomainEvent(new CreateFirstLevelCoaDomainEvent(coa.HeadName, coa.Type));
            return coa;
        }
        catch (Exception ex)
        {
            return Result.Failure<Accounts>(new Error("Accounts.Create", $"Failed to create Accounts: {ex.Message}"));
        }
    }

    private static string GenerateNextHeadCode(Accounts parentCoa)
    {
        Guard.Against.Null(parentCoa, nameof(parentCoa));

        var parentChildCodes = parentCoa._accounts
            .Where(coa => coa.HeadCode.Length == parentCoa.HeadCode.Length + 2)
            .Select(coa => int.Parse(coa.HeadCode.Substring(parentCoa.HeadCode.Length)))
            .ToList();

        int nextChildCode = parentChildCodes.Any() ? parentChildCodes.Max() + 1 : 1;
        return $"{parentCoa.HeadCode}{nextChildCode:D2}";
    }

    public void InsertChildrens(List<Accounts> childCOAs)
    {
        Guard.Against.Null(childCOAs, nameof(childCOAs));
        _accounts.AddRange(childCOAs);
    }
}
