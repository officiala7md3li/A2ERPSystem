using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Entities.Transactions;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Shared.Results;
using FluentAssertions;
using Xunit;

namespace DomainDrivenERP.Application.UnitTests.Features.Entities;

public class AccountsEntityTests
{
    #region Accounts.Create (First Level) — Uses Guid Id

    [Fact]
    public void Create_FirstLevel_Should_Generate_Guid_Id()
    {
        // Act
        Result<Accounts> result = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBe(Guid.Empty, "First-level account should have a non-empty Guid Id");
        result.Value.HeadCode.Should().Be("01");
        result.Value.HeadName.Should().Be("Assets");
        result.Value.HeadLevel.Should().Be(1);
        result.Value.ParentAccountId.Should().BeNull("First-level account has no parent");
        result.Value.ParentHeadCode.Should().BeNull("First-level account has no parent HeadCode");
    }

    [Fact]
    public void Create_FirstLevel_Should_Fail_When_HeadCode_Empty()
    {
        // Act & Assert — Guard clause throws exception for empty HeadCode
        Assert.ThrowsAny<Exception>(() => Accounts.Create("Assets", "", ChartOfAccountsType.Assets));
    }

    [Fact]
    public void Create_FirstLevel_Should_Fail_When_HeadName_Empty()
    {
        // Act & Assert — Guard clause throws exception for empty HeadName
        Assert.ThrowsAny<Exception>(() => Accounts.Create("", "01", ChartOfAccountsType.Assets));
    }

    [Fact]
    public void Create_FirstLevel_Should_Generate_Unique_Ids()
    {
        // Act
        Result<Accounts> result1 = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Result<Accounts> result2 = Accounts.Create("Liabilities", "02", ChartOfAccountsType.Liabilities);

        // Assert
        result1.Value.Id.Should().NotBe(result2.Value.Id, "Each account should have a unique Guid");
    }

    #endregion

    #region Accounts.Create (Child) — Parent FK via Guid

    [Fact]
    public void Create_Child_Should_Set_ParentAccountId_As_Guid()
    {
        // Arrange
        Result<Accounts> parentResult = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Accounts parent = parentResult.Value;

        // Act
        Result<Accounts> childResult = Accounts.Create("Current Assets", parent);

        // Assert
        childResult.IsSuccess.Should().BeTrue();
        childResult.Value.Id.Should().NotBe(Guid.Empty);
        childResult.Value.ParentAccountId.Should().Be(parent.Id, "Child should reference parent by Guid Id");
        childResult.Value.ParentHeadCode.Should().Be(parent.HeadCode, "Child should still store parent HeadCode for reference");
        childResult.Value.HeadLevel.Should().Be(2);
        childResult.Value.Type.Should().Be(parent.Type, "Child inherits parent type");
    }

    [Fact]
    public void Create_Child_Should_Generate_Hierarchical_HeadCode()
    {
        // Arrange
        Result<Accounts> parentResult = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Accounts parent = parentResult.Value;

        // Act
        Result<Accounts> child1Result = Accounts.Create("Current Assets", parent);
        Result<Accounts> child2Result = Accounts.Create("Non-Current Assets", parent);

        // Assert
        child1Result.Value.HeadCode.Should().Be("0101");
        child2Result.Value.HeadCode.Should().Be("0102");
    }

    [Fact]
    public void Create_Child_Should_Fail_When_Parent_Is_Null()
    {
        // Act & Assert — Guard throws for null parent
        Assert.ThrowsAny<Exception>(() => Accounts.Create("Current Assets", null));
    }

    [Fact]
    public void Create_Child_Should_Add_To_Parent_ChildAccounts_Collection()
    {
        // Arrange
        Result<Accounts> parentResult = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Accounts parent = parentResult.Value;

        // Act
        Result<Accounts> childResult = Accounts.Create("Current Assets", parent);

        // Assert
        parent.ChildAccounts.Should().HaveCount(1);
        parent.ChildAccounts.First().Id.Should().Be(childResult.Value.Id);
        parent.ChildAccounts.First().ParentAccountId.Should().Be(parent.Id);
    }

    [Fact]
    public void Create_GrandChild_Should_Chain_ParentAccountId_Correctly()
    {
        // Arrange
        Result<Accounts> rootResult = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Accounts root = rootResult.Value;
        Result<Accounts> childResult = Accounts.Create("Current Assets", root);
        Accounts child = childResult.Value;

        // Act
        Result<Accounts> grandChildResult = Accounts.Create("Cash", child);

        // Assert
        grandChildResult.IsSuccess.Should().BeTrue();
        grandChildResult.Value.ParentAccountId.Should().Be(child.Id, "GrandChild points to child by Id");
        grandChildResult.Value.ParentAccountId.Should().NotBe(root.Id, "GrandChild does NOT point to root");
        grandChildResult.Value.HeadLevel.Should().Be(3);
        grandChildResult.Value.HeadCode.Should().Be("010101");
    }

    #endregion

    #region Accounts.InsertChildrens

    [Fact]
    public void InsertChildrens_Should_Add_Children_To_Collection()
    {
        // Arrange
        Result<Accounts> parentResult = Accounts.Create("Assets", "01", ChartOfAccountsType.Assets);
        Accounts parent = parentResult.Value;
        Result<Accounts> child1 = Accounts.Create("C1", "0101", ChartOfAccountsType.Assets);
        Result<Accounts> child2 = Accounts.Create("C2", "0102", ChartOfAccountsType.Assets);

        // Act
        parent.InsertChildrens(new List<Accounts> { child1.Value, child2.Value });

        // Assert
        parent.ChildAccounts.Should().HaveCount(2);
    }

    #endregion
}
