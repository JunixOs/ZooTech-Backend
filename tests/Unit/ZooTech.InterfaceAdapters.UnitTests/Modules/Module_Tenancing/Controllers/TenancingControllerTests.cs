using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
        var mediatorMock = new Mock<IMediator>();
        var expectedResult = new CreateTenantResult
        {
            Code = "tenant-01",
            SubDomain = "tenant-01",
            DisplayName = "Display",
            LegalName = "Legal"
        };

        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTenantCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new TenancingController(mediatorMock.Object);

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

        mediatorMock.Verify(m => m.Send(It.IsAny<CreateTenantCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
