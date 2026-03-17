using DomainDrivenERP.Domain.Dtos;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Entities.Journals;
using DomainDrivenERP.Domain.Entities.Transactions;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Shared.Results;
using FluentAssertions;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class JournalEntityTests
{
    #region Journal.Create with TransactionDto using AccountId (Guid)

    [Fact]
    public void Journal_Create_Should_Create_Transactions_With_Guid_COAId()
    {
        // Arrange
        Guid accountId1 = Guid.NewGuid();
        Guid accountId2 = Guid.NewGuid();

        var transactions = new List<TransactionDto>
        {
            new TransactionDto
            {
                AccountId = accountId1,
                AccountName = "Cash",
                AccountHeadCode = "0101",
                Debit = 1000,
                Credit = 1000
            },
            new TransactionDto
            {
                AccountId = accountId2,
                AccountName = "Revenue",
                AccountHeadCode = "0201",
                Debit = 500,
                Credit = 500
            }
        };

        // Act
        Result<Journal> result = Journal.Create("Test Journal", false, DateTime.UtcNow, transactions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Transactions.Should().HaveCount(2);

        var transactionList = result.Value.Transactions.ToList();
        transactionList[0].COAId.Should().Be(accountId1, "Transaction should store COAId as Guid");
        transactionList[1].COAId.Should().Be(accountId2, "Transaction should store COAId as Guid");
    }

    [Fact]
    public void Journal_Create_Transactions_COAId_Should_Be_Guid_Type()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();

        var transactions = new List<TransactionDto>
        {
            new TransactionDto
            {
                AccountId = accountId,
                Debit = 500,
                Credit = 500
            }
        };

        // Act
        Result<Journal> result = Journal.Create("Test", false, DateTime.UtcNow, transactions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Transaction txn = result.Value.Transactions.First();
        txn.COAId.GetType().Should().Be(typeof(Guid), "COAId should be Guid, not string");
    }

    #endregion

    #region Journal Snapshot Roundtrip

    [Fact]
    public void Journal_Snapshot_Should_Preserve_Guid_COAIds()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        var transactions = new List<TransactionDto>
        {
            new TransactionDto
            {
                AccountId = accountId,
                Debit = 100,
                Credit = 100
            }
        };

        Result<Journal> journalResult = Journal.Create("Snapshot Test", false, DateTime.UtcNow, transactions);
        Journal journal = journalResult.Value;

        // Act
        JournalSnapshot snapshot = journal.ToSnapshot();
        Journal restored = Journal.FromSnapshot(snapshot);

        // Assert
        restored.Transactions.Should().HaveCount(1);
        restored.Transactions.First().COAId.Should().Be(accountId, "COAId should survive snapshot roundtrip as Guid");
    }

    #endregion
}
