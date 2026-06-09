using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Application.UnitTests.Modules.Module_Tenancing.UseCases.CreateTenant;

public class CreateTenantHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Result_When_Provisioning_Succeeds()
    {
        // Arrange
        var provisioningServiceMock = new Mock<ITenantProvisioningService>();
        provisioningServiceMock
            .Setup(x => x.ProvisionAsync(It.IsAny<CreateTenantCommand>()))
            .ReturnsAsync(true);

        var handler = new CreateTenantHandler(provisioningServiceMock.Object);
        var command = TenantTestDataFactory.CreateValidCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(command.Code);
        result.SubDomain.Should().Be(command.SubDomain);
        result.DisplayName.Should().Be(command.DisplayName);
        result.LegalName.Should().Be(command.LegalName);
    }

    [Fact]
    public async Task Handle_Should_Throw_TenantProvisioningException_When_Provisioning_Fails()
    {
        // Arrange
        var provisioningServiceMock = new Mock<ITenantProvisioningService>();
        provisioningServiceMock
            .Setup(x => x.ProvisionAsync(It.IsAny<CreateTenantCommand>()))
            .ReturnsAsync(false);

        var handler = new CreateTenantHandler(provisioningServiceMock.Object);
        var command = TenantTestDataFactory.CreateValidCommand();

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<TenantProvisioningException>();
    }
}
