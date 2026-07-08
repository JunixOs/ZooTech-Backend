using FluentAssertions;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Domain.Shared.ValueObjects;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;
using ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb;

namespace ZooTech.Infrastructure.UnitTests.Persistence.Mappers.MainTenantsDb;

public class TenantMapperTests
{
    [Fact]
    public void ToDomain_Should_Map_All_Fields()
    {
        // Arrange
        var entity = new tenant
        {
            id = 1,
            code = "CODE1",
            subdomain = "code1",
            display_name = "Display",
            legal_name = "Legal SAC",
            email = "a@b.com",
            phone = "123456",
            status = "TRIAL",
            created_at = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            updated_at = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var domain = TenantMapper.ToDomain(entity);

        // Assert
        domain.Id.Should().Be(1);
        domain.Code.Should().Be("CODE1");
        domain.SubDomain.Should().Be("code1");
        domain.DisplayName.Should().Be("Display");
        domain.LegalName.Should().Be("Legal SAC");
        domain.Email.Value.Should().Be("a@b.com");
        domain.Phone.Should().Be("123456");
        domain.Status.Should().Be(TenantStatus.TRIAL);
        domain.CreatedAt.Should().Be(entity.created_at.Value);
        domain.UpdatedAt.Should().Be(entity.updated_at.Value);
    }

    [Fact]
    public void ToDomain_Should_Default_UpdatedAt_When_Null()
    {
        // Arrange
        var entity = new tenant
        {
            id = 1,
            code = "C",
            subdomain = "c",
            display_name = "D",
            legal_name = "L",
            email = "e@f.com",
            phone = "1",
            status = "ACTIVE",
            created_at = DateTime.UtcNow,
            updated_at = null
        };

        // Act
        var domain = TenantMapper.ToDomain(entity);

        // Assert
        domain.UpdatedAt.Should().Be(default);
    }

    [Fact]
    public void ToEntity_Should_Map_All_Fields()
    {
        // Arrange
        var domain = TenantDomainEntity.Create(
            id: 2,
            code: "CODE2",
            subDomain: "code2",
            displayName: "Display2",
            legalName: "Legal2 SAC",
            email: new Email("b@c.com"),
            phone: "654321",
            status: TenantStatus.ACTIVE,
            createdAt: new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            updatedAt: new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc)
        );

        // Act
        var entity = TenantMapper.ToOrm(domain);

        // Assert
        entity.code.Should().Be("CODE2");
        entity.subdomain.Should().Be("code2");
        entity.display_name.Should().Be("Display2");
        entity.legal_name.Should().Be("Legal2 SAC");
        entity.email.Should().Be("b@c.com");
        entity.phone.Should().Be("654321");
        entity.status.Should().Be("ACTIVE");
        entity.created_at.Should().Be(domain.CreatedAt);
        entity.updated_at.Should().Be(domain.UpdatedAt);
    }
}
