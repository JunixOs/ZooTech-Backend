using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Infrastructure.Context;
namespace ZooTech.API.IntegrationTests;

public class TenantProvisioningIntegrationTests : IClassFixture<ZooTechApiFactory>
{
    private readonly HttpClient _client;

    public TenantProvisioningIntegrationTests(ZooTechApiFactory factory)
    {
        var provisioningMock = new Mock<ITenantProvisioningService>();
        provisioningMock
            .Setup(x => x.ProvisionAsync(It.IsAny<CreateTenantCommand>()));

        var storeMock = new Mock<ITenantStore>();
        storeMock
            .Setup(x => x.GetBySubDomainAsync(It.IsAny<string>()))
            .ReturnsAsync(new TenantInfo
            {
                Id = 1,
                SubDomain = "test",
                Code = "TEST",
                DatabaseName = "ZooTech_test_Db",
                Status = TenantStatus.ACTIVE,
                Email = "test@test.com"
            });

        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("AllowedHosts", "*");
                builder.UseSetting("MultiTenant:BaseDomain", "zootech.com");
                builder.UseSetting("Frontend:FrontendPort", "5000");
                builder.UseSetting("Frontend:FrontendIP", "localhost");
                builder.UseSetting("Frontend:FrontendProtocol", "http");

                builder.ConfigureTestServices(services =>
                {
                    var provDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITenantProvisioningService));
                    if (provDescriptor != null) services.Remove(provDescriptor);
                    services.AddSingleton(provisioningMock.Object);

                    var storeDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITenantStore));
                    if (storeDescriptor != null) services.Remove(storeDescriptor);
                    services.AddSingleton(storeMock.Object);

                    var ctxDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITenantContext));
                    if (ctxDescriptor != null) services.Remove(ctxDescriptor);
                    services.AddSingleton<ITenantContext>(new TenantContext());
                });
            })
            .CreateClient();
    }

    [Fact]
    public async Task POST_Tenancing_Should_Return_200_When_Provisioning_Succeeds()
    {
        // Arrange
        var payload = new
        {
            Code = "tenant-int",
            SubDomain = "tenant-int",
            DisplayName = "Integration Tenant",
            LegalName = "Integration Tenant SAC",
            Email = "int@test.com",
            Phone = "+51111111111",
            TimeZone = "UTC",
            TenantAddress = new
            {
                Country = "Perú",
                State = "Lima",
                Province = "Lima",
                City = "Lima",
                AddressLine1 = "Av. Int 123",
                AddressLine2 = ""
            },
            TenantBranding = new
            {
                PrimaryColor = "#000000",
                SecondaryColor = "#FFFFFF",
                LogoUrl = ""
            },
            TenantDatabaseConnection = new { IsActive = true }
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/tenancing")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Host = "test.zootech.com";
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
