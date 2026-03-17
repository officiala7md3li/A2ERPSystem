using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Entities.Transactions;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Shared.Results;
using FluentAssertions;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

/// <summary>
/// Integration-style tests that verify the full relationship chain:
/// Accounts → Transaction → Journal all using Guid Ids.
/// </summary>
public class EntityRelationshipTests
{
    #region Accounts Entity Uses Guid Id (not HeadCode) as Primary Identifier

    [Fact]
    public void Account_Id_Should_Be_Guid_Not_HeadCode()
    {
        // Act
        Result<Accounts> result = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);

        // Assert
        result.Value.Id.Should().NotBe(Guid.Empty);
        result.Value.Id.GetType().Should().Be(typeof(Guid));
        result.Value.HeadCode.Should().Be("01", "HeadCode remains as business identifier");
    }

    #endregion

    #region Parent-Child Relationship Uses Guid ParentAccountId

    [Fact]
    public void ParentChild_Relationship_Should_Use_Guid_Not_HeadCode()
    {
        // Arrange
        Result<Accounts> parentResult = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Accounts parent = parentResult.Value;

        // Act
        Result<Accounts> childResult = Accounts.Create("Cash", parent);

        // Assert
        Accounts child = childResult.Value;
        child.ParentAccountId.Should().Be(parent.Id, "FK should point to parent's Guid Id");
        child.ParentAccountId.Should().NotBeNull();
        child.ParentHeadCode.Should().Be("01", "HeadCode is still kept for reference");

        // Verify the relationship is through Id, not HeadCode
        parent.ChildAccounts.Should().Contain(c => c.Id == child.Id);
    }

    #endregion

    #region Transaction-Account Relationship Uses Guid COAId

    [Fact]
    public void Transaction_COAId_Should_Reference_Account_By_Guid_Id()
    {
        // Arrange
        Result<Accounts> accountResult = Accounts.Create("Cash", "0101", ChartOfAccountsType.Assets);
        Accounts account = accountResult.Value;

        // Act — create transaction referencing the account by Guid
        var snapshot = new TransactionSnapshot
        {
            TransactionId = Guid.NewGuid(),
            JournalId = Guid.NewGuid(),
            COAId = account.Id,  // Using Guid Id, not HeadCode string
            Debit = 1000.0,
            Credit = 500.0
        };
        Transaction transaction = Transaction.FromSnapshot(snapshot);

        // Assert
        transaction.COAId.Should().Be(account.Id, "Transaction.COAId should be the Account's Guid Id");
        transaction.COAId.Should().NotBe(Guid.Empty);
    }

    #endregion

    #region Full Chain: Root → Child → GrandChild with Consistent Guid Ids

    [Fact]
    public void Full_Hierarchy_Should_Use_Guid_Ids_Throughout()
    {
        // Arrange & Act
        Result<Accounts> rootResult = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Accounts root = rootResult.Value;

        Result<Accounts> childResult = Accounts.Create("Current Assets", root);
        Accounts child = childResult.Value;

        Result<Accounts> grandchildResult = Accounts.Create("Cash", child);
        Accounts grandchild = grandchildResult.Value;

        // Assert — each level has unique Guid and parent references are Guid-based
        root.Id.Should().NotBe(child.Id);
        root.Id.Should().NotBe(grandchild.Id);
        child.Id.Should().NotBe(grandchild.Id);

        root.ParentAccountId.Should().BeNull("Root has no parent");
        child.ParentAccountId.Should().Be(root.Id, "Child → Root via Guid");
        grandchild.ParentAccountId.Should().Be(child.Id, "Grandchild → Child via Guid");

        // HeadCodes are consistent but independent from relationships
        root.HeadCode.Should().Be("01");
        child.HeadCode.Should().Be("0101");
        grandchild.HeadCode.Should().Be("010101");
    }

    #endregion

    #region No String-Based Foreign Keys

    [Fact]
    public void Account_ParentAccountId_Property_Should_Be_Nullable_Guid()
    {
        var accountType = typeof(Accounts);
        var prop = accountType.GetProperty("ParentAccountId");

        prop.Should().NotBeNull();
        prop!.PropertyType.Should().Be(typeof(Guid?), "ParentAccountId must be nullable Guid, not string");
    }

    [Fact]
    public void Transaction_COAId_Property_Should_Be_Guid()
    {
        var transactionType = typeof(Transaction);
        var prop = transactionType.GetProperty("COAId");

        prop.Should().NotBeNull();
        prop!.PropertyType.Should().Be(typeof(Guid), "COAId must be Guid, not string");
    }

    #endregion
}
