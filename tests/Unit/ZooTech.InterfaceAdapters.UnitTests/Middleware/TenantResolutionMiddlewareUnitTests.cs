using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Middleware;

namespace ZooTech.InterfaceAdapters.UnitTests.Middleware
{
    public class TenantResolutionMiddlewareUnitTests
    {
        private static IConfiguration CreateConfig(string baseDomain = "zootech.com", string adminSubDomain = "admin")
        {
            var settings = new Dictionary<string, string>
            {
                { "MultiTenant:BaseDomain", baseDomain },
                { "MultiTenant:AdminSubDomain", adminSubDomain }
            };
            return new ConfigurationBuilder().AddInMemoryCollection(settings!).Build();
        }

        [Fact]
        public async Task Resolve_Existing_tenant1_Domain()
        {
            // Arrange
            var configuration = CreateConfig();
            var tenantStoreMock = new Mock<ITenantStore>();
            var tenantContextMock = new Mock<ITenantContext>();

            var tenant = new TenantInfo
            {
                Id = 1,
                SubDomain = "tenant1",
                LegalName = "tenant S.A.C",
                DisplayName = "Tenant 1",
                Code = "TENANT_1",
                DatabaseName = "ZooTech_tenant1_Db",
                IsDatabaseActive = true,
                Status = TenantStatus.ACTIVE,
                Email = "tenant1@gmail.com"
            };

            tenantStoreMock
                .Setup(x => x.GetBySubDomainAsync("tenant1"))
                .ReturnsAsync(tenant);

            var nextCalled = false;
            RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

            var middleware = new TenantResolutionMiddleware(next, configuration);
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Tenant-Url"] = "tenant1.zootech.com";

            // Act
            await middleware.InvokeAsync(context, tenantStoreMock.Object, tenantContextMock.Object);

            // Assert
            nextCalled.Should().BeTrue();
            tenantContextMock.Verify(
                x => x.SetTenant(tenant.Id, tenant.Code, tenant.LegalName, tenant.DisplayName, TenantType.Tenant, tenant.SubDomain, tenant.DatabaseName),
                Times.Once
            );
        }

        [Fact]
        public async Task Resolve_NonExisting_localhost_DomainAsync()
        {
            // Arrange
            var configuration = CreateConfig();
            var tenantStoreMock = new Mock<ITenantStore>();
            var tenantContextMock = new Mock<ITenantContext>();

            var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask, configuration);
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("localhost");

            // Act
            var act = () => middleware.InvokeAsync(context, tenantStoreMock.Object, tenantContextMock.Object);

            // Assert
            // NotFoundException is translated to a 404 response by ExceptionHandlingMiddleware,
            // which wraps TenantResolutionMiddleware in the real pipeline (see Program.cs).
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
