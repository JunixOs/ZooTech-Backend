using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports;

namespace ZooTech.Application.UnitTests.Modules.Module_Tenancing.UseCases.CreateTenant;

public class CreateTenantInteractorTests
{
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<ITenantProvisioningService> _tenantProvisioningServiceMock;
    private readonly Mock<ICreateTenantOutputPort> _createTenantOutputPortMock;
    private readonly CreateTenantInteractor _interactor;

    public CreateTenantInteractorTests()
    {
        _tenantContextMock = new Mock<ITenantContext>();
        _tenantProvisioningServiceMock = new Mock<ITenantProvisioningService>();
        _createTenantOutputPortMock = new Mock<ICreateTenantOutputPort>();

        _interactor = new CreateTenantInteractor(
            _tenantContextMock.Object,
            _tenantProvisioningServiceMock.Object,
            _createTenantOutputPortMock.Object);
    }

    [Fact]
    public async Task Handle_CuandoProvisionamientoFalla_DebeLanzarTenantProvisioningException()
    {
        // Arrange
        var cmd = new CreateTenantCommand { Code = "TEST", SubDomain = "test", DisplayName = "Test", LegalName = "Test Corp" };

        _tenantProvisioningServiceMock.Setup(x => x.ProvisionAsync(cmd))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _interactor.Handle(cmd);

        // Assert
        await act.Should().ThrowAsync<TenantProvisioningException>();
        _createTenantOutputPortMock.Verify(x => x.Ok(It.IsAny<CreateTenantOutput>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CuandoDatosSonValidosYProvisionamientoExitoso_DebeNotificarOutputPort()
    {
        // Arrange
        var cmd = new CreateTenantCommand { Code = "TEST", SubDomain = "test", DisplayName = "Test", LegalName = "Test Corp" };

        _tenantProvisioningServiceMock.Setup(x => x.ProvisionAsync(cmd))
            .ReturnsAsync(true);

        // Act
        await _interactor.Handle(cmd);

        // Assert
        _createTenantOutputPortMock.Verify(x => x.Ok(It.Is<CreateTenantOutput>(o => 
            o.Code == cmd.Code && 
            o.SubDomain == cmd.SubDomain && 
            o.DisplayName == cmd.DisplayName && 
            o.LegalName == cmd.LegalName)), Times.Once);
    }
}
