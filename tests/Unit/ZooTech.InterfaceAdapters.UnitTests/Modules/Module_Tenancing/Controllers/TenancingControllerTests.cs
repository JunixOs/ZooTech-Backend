using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_Tenancing;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Tenancing.Controllers;

public class TenancingControllerTests
{
    [Fact]
    public async Task CreateTenant_Should_Send_Command_And_Return_Ok()
    {
        // Arrange
        var expectedResult = new CreateTenantOutput
        {
            Code = "tenant-01",
            SubDomain = "tenant-01",
            DisplayName = "Display",
            LegalName = "Legal"
        };

        var pipeline = new BehaviorPipeline<CreateTenantCommand, CreateTenantOutput>(
            [],
            _ => Task.FromResult(expectedResult)
        );

        var pipelineFactoryMock = new Mock<ICreateTenantPipelineFactory>();
        pipelineFactoryMock
            .Setup(bm => bm.Create())
            .Returns(pipeline);

        var controller = new TenancingController(pipelineFactoryMock.Object);

        var request = new CreateTenantRequestDto
        {
            Code = "tenant-01",
            SubDomain = "tenant-01",
            DisplayName = "Display",
            LegalName = "Legal",
            Email = "a@b.com",
            Phone = "1",
            TimeZone = "UTC",
            TenantAddress = new TenantAddressRequestDto
            {
                Country = "C",
                State = "S",
                Province = "P",
                City = "C",
                AddressLine1 = "A1"
            },
            TenantBranding = new TenantBrandingRequestDto
            {
                PrimaryColor = "#000000",
                SecondaryColor = "#FFFFFF",
                LogoUrl = ""
            },
            TenantDatabaseConnection = new TenantDatabaseConnectionRequestDto { IsActive = true }
        };

        // Act
        var result = await controller.CreateTenant(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var responseDto = okResult.Value.Should().BeOfType<CreateTenantResponseDto>().Subject;
        responseDto.Code.Should().Be(expectedResult.Code);
        responseDto.SubDomain.Should().Be(expectedResult.SubDomain);

        pipelineFactoryMock.Verify(bm => bm.Create(), Times.Once);
    }
}
