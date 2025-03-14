using System.Threading;
using System.Threading.Tasks;
using DomainDrivenERP.Application.Features.Coas.Commands.CreateCoa;
using DomainDrivenERP.Domain.Entities.COAs;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Presentation.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DomainDrivenERP.Tests.Controllers
{
    public class AccountsControllerTests
    {
        private readonly Mock<ISender> _senderMock;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _senderMock = new Mock<ISender>();
            _controller = new AccountsController(_senderMock.Object);
        }

        [Fact]
        public async Task CreateCoa_ReturnsCustomResult()
        {
            // Arrange
            var createCoaCommand = new CreateCoaCommand
            {
                HeadName = "Test Account",
                ParentHeadCode = "001",
                IsGl = true,
                Type = ChartOfAccountsType.Assets
            };

            var accounts = new Accounts
            {
                HeadCode = "002",
                HeadName = "Test Account",
                ParentHeadCode = "001",
                IsGl = true,
                Type = ChartOfAccountsType.Assets,
                HeadLevel = 2,
                IsActive = true
            };

            var result = Result.Success(accounts);
            _senderMock.Setup(s => s.Send(It.IsAny<CreateCoaCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(result);

            // Act
            var response = await _controller.CreateCoa(createCoaCommand, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(response);
            var returnedResult = Assert.IsType<Result<Accounts>>(actionResult.Value);
            Assert.True(returnedResult.IsSuccess);
            Assert.Equal("Test Account", returnedResult.Value.HeadName);
        }
    }
}
