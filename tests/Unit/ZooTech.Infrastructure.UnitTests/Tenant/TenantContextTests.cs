using FluentAssertions;
using Xunit;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Tenant;

public class TenantContextTests
{
    [Fact]
    public void SetTenant_DebeAsignarPropiedadesCorrectamente()
    {
        // Arrange
        var context = new TenantContext();
        
        long expectedId = 100;
        string expectedCode = "TENANT100";
        string expectedSubDomain = "mi-subdominio";
        string expectedDbName = "db_tenant_100";

        // Act
        context.SetTenant(expectedId, expectedCode, expectedSubDomain, expectedDbName);

        // Assert
        context.TenantId.Should().Be(expectedId);
        context.Code.Should().Be(expectedCode);
        context.SubDomain.Should().Be(expectedSubDomain);
        context.DatabaseName.Should().Be(expectedDbName);
    }
}
