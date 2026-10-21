using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Administracion.Controllers;

public class TenantManagementControllerTests
{
    [Fact]
    public async Task CreateTenant_Should_Send_Command_And_Return_Ok()
    {
        var expectedResult = new CreateTenantOutput
        {
            Code = "tenant-01",
            SubDomain = "tenant-01",
            DisplayName = "Display",
            LegalName = "Legal"
        };

        CreateTenantCommand? capturedCommand = null;

        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<CreateTenantCommand, CreateTenantOutput>(
            (command, _) =>
            {
                capturedCommand = command;

                return Task.FromResult(expectedResult);
            });

        var controller = new TenantManagementController(dispatcher);

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

            TenantDatabaseConnection =
                new TenantDatabaseConnectionRequestDto
                {
                    IsActive = true
                }
        };

        var result = await controller.CreateTenant(request);

        var okResult =
            result.Should()
                .BeOfType<OkObjectResult>()
                .Subject;

        var responseDto =
            okResult.Value
                .Should()
                .BeOfType<CreateTenantResponseDto>()
                .Subject;

        responseDto.Code.Should().Be(expectedResult.Code);
        responseDto.SubDomain.Should().Be(expectedResult.SubDomain);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.Code.Should().Be(request.Code);
        capturedCommand.SubDomain.Should().Be(request.SubDomain);
        capturedCommand.DisplayName.Should().Be(request.DisplayName);
        capturedCommand.LegalName.Should().Be(request.LegalName);
    }

    private sealed class TestBehaviorDispatcher : IBehaviorDispatcher
    {
        private readonly Dictionary<
            (Type Request, Type Response),
            Func<object, CancellationToken, Task<object>>> _handlers = new();

        public void Setup<TRequest, TResponse>(
            Func<TRequest, CancellationToken, Task<TResponse>> handler)
        {
            _handlers[(typeof(TRequest), typeof(TResponse))] =
                async (request, cancellationToken) =>
                    await handler(
                        (TRequest)request,
                        cancellationToken);
        }

        public async Task<TResponse> Send<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_handlers.TryGetValue(
                    (typeof(TRequest), typeof(TResponse)),
                    out var handler))
            {
                throw new InvalidOperationException(
                    $"No test handler configured for " +
                    $"{typeof(TRequest).Name} -> " +
                    $"{typeof(TResponse).Name}");
            }

            var response = await handler(
                request!,
                cancellationToken);

            return (TResponse)response;
        }
    }
}
