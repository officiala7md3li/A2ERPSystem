using DomainDrivenERP.Domain.Dtos;
using FluentAssertions;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class DtoTests
{
    #region TransactionDto has AccountId (Guid)

    [Fact]
    public void TransactionDto_Should_Have_AccountId_Property_Of_Type_Guid()
    {
        // Arrange & Act
        var dto = new TransactionDto();

        // Assert
        dto.GetType().GetProperty("AccountId")!.PropertyType.Should().Be(typeof(Guid));
    }

    [Fact]
    public void TransactionDto_Should_Store_Guid_AccountId()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();

        // Act
        var dto = new TransactionDto
        {
            AccountId = accountId,
            AccountName = "Cash",
            AccountHeadCode = "0101",
            Debit = 100,
            Credit = 0
        };

        // Assert
        dto.AccountId.Should().Be(accountId);
        dto.AccountName.Should().Be("Cash");
        dto.AccountHeadCode.Should().Be("0101");
    }

    #endregion

    #region JournalTransactionsDto has AccountId (Guid)

    [Fact]
    public void JournalTransactionsDto_Should_Have_AccountId_Property_Of_Type_Guid()
    {
        // Arrange & Act
        var dto = new JournalTransactionsDto();

        // Assert
        dto.GetType().GetProperty("AccountId")!.PropertyType.Should().Be(typeof(Guid));
    }

    [Fact]
    public void JournalTransactionsDto_Should_Store_All_Fields()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Guid transactionId = Guid.NewGuid();
        Guid journalId = Guid.NewGuid();

        // Act
        var dto = new JournalTransactionsDto
        {
            TransactionId = transactionId,
            JournalId = journalId,
            AccountId = accountId,
            AccountName = "Revenue",
            AccountHeadCode = "0201",
            Debit = 500,
            Credit = 0
        };

        // Assert
        dto.TransactionId.Should().Be(transactionId);
        dto.JournalId.Should().Be(journalId);
        dto.AccountId.Should().Be(accountId);
        dto.AccountName.Should().Be("Revenue");
        dto.AccountHeadCode.Should().Be("0201");
    }

    #endregion

    #region CoaWithChildrenDto has Id (Guid) and ParentAccountId (Guid?)

    [Fact]
    public void CoaWithChildrenDto_Should_Have_Id_And_ParentAccountId()
    {
        // Arrange & Act
        var dto = new CoaWithChildrenDto();

        // Assert
        dto.GetType().GetProperty("Id")!.PropertyType.Should().Be(typeof(Guid));
        dto.GetType().GetProperty("ParentAccountId")!.PropertyType.Should().Be(typeof(Guid?));
    }

    [Fact]
    public void CoaWithChildrenDto_Should_Store_Guid_Based_Relationships()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid parentId = Guid.NewGuid();

        // Act
        var dto = new CoaWithChildrenDto
        {
            Id = id,
            HeadCode = "0101",
            HeadName = "Current Assets",
            ParentAccountId = parentId,
            ParentHeadCode = "01",
            HeadLevel = 2,
            IsGl = false,
            IsActive = true
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.ParentAccountId.Should().Be(parentId);
        dto.HeadCode.Should().Be("0101");
        dto.ParentHeadCode.Should().Be("01");
    }

    [Fact]
    public void CoaWithChildrenDto_RootAccount_Should_Have_Null_ParentAccountId()
    {
        // Arrange & Act
        var dto = new CoaWithChildrenDto
        {
            Id = Guid.NewGuid(),
            HeadCode = "01",
            HeadName = "Assets",
            ParentAccountId = null,
            ParentHeadCode = null,
            HeadLevel = 1
        };

        // Assert
        dto.ParentAccountId.Should().BeNull();
    }

    #endregion
}
