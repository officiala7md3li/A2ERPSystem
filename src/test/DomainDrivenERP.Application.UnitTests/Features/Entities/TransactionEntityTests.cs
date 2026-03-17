using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Entities.Transactions;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Shared.Results;
using FluentAssertions;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class TransactionEntityTests
{
    #region Transaction COAId is Guid

    [Fact]
    public void Transaction_COAId_Should_Be_Guid()
    {
        // Arrange
        Guid transactionId = Guid.NewGuid();
        Guid journalId = Guid.NewGuid();
        Guid coaId = Guid.NewGuid();

        // Act
        var snapshot = new TransactionSnapshot
        {
            TransactionId = transactionId,
            JournalId = journalId,
            COAId = coaId,
            Debit = 100.0,
            Credit = 50.0
        };

        Transaction transaction = Transaction.FromSnapshot(snapshot);

        // Assert
        transaction.COAId.Should().Be(coaId, "COAId should be a Guid referencing the Account's Id");
        transaction.COAId.GetType().Should().Be(typeof(Guid));
        transaction.TransactionId.Should().Be(transactionId);
        transaction.JournalId.Should().Be(journalId);
    }

    [Fact]
    public void Transaction_Snapshot_Should_Roundtrip_Guid_COAId()
    {
        // Arrange
        Guid coaId = Guid.NewGuid();
        var snapshot = new TransactionSnapshot
        {
            TransactionId = Guid.NewGuid(),
            JournalId = Guid.NewGuid(),
            COAId = coaId,
            Debit = 500.0,
            Credit = 250.0
        };

        // Act
        Transaction transaction = Transaction.FromSnapshot(snapshot);
        TransactionSnapshot roundTrip = transaction.ToSnapshot();

        // Assert
        roundTrip.COAId.Should().Be(coaId, "COAId should survive roundtrip as Guid");
        roundTrip.TransactionId.Should().Be(snapshot.TransactionId);
        roundTrip.JournalId.Should().Be(snapshot.JournalId);
        roundTrip.Debit.Should().Be(500.0);
        roundTrip.Credit.Should().Be(250.0);
    }

    #endregion

    #region TransactionSnapshot COAId type

    [Fact]
    public void TransactionSnapshot_COAId_Should_Be_Guid_Type()
    {
        // Arrange & Act
        var snapshot = new TransactionSnapshot();

        // Assert — verify the property type is Guid, not string
        snapshot.GetType().GetProperty("COAId")!.PropertyType.Should().Be(typeof(Guid));
    }

    #endregion
}
