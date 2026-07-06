using FluentAssertions;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Domain.Shared.ValueObjects;

namespace ZooTech.Domain.UnitTests.Admin.Entities;

public class TenantDomainEntityTests
{
    [Fact]
    public void Create_Should_Initialize_All_Properties()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var tenant = TenantDomainEntity.Create(
            id: 1,
            code: "CODE123",
            subDomain: "code123",
            displayName: "Display",
            legalName: "Legal Name SAC",
            email: new Email("a@b.com"),
            phone: "123456",
            status: TenantStatus.ACTIVE,
            createdAt: now,
            updatedAt: now
        );

        // Assert
        tenant.Id.Should().Be(1);
        tenant.Code.Should().Be("CODE123");
        tenant.SubDomain.Should().Be("code123");
        tenant.DisplayName.Should().Be("Display");
        tenant.LegalName.Should().Be("Legal Name SAC");
        tenant.Email.Value.Should().Be("a@b.com");
        tenant.Phone.Should().Be("123456");
        tenant.Status.Should().Be(TenantStatus.ACTIVE);
        tenant.CreatedAt.Should().Be(now);
        tenant.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public void Create_Should_Default_CreatedAt_To_UtcNow_When_Null()
    {
        // Act
        var tenant = TenantDomainEntity.Create(
            id: 0,
            code: "C",
            subDomain: "c",
            displayName: "D",
            legalName: "L",
            email: new Email("e@f.com"),
            phone: "1",
            status: TenantStatus.TRIAL,
            createdAt: null,
            updatedAt: DateTime.UtcNow
        );

        // Assert
        tenant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateContactInfo_Should_Change_Email_And_Phone_And_UpdatedAt()
    {
        // Arrange
        var tenant = TenantDomainEntity.Create(
            id: 1,
            code: "C",
            subDomain: "c",
            displayName: "D",
            legalName: "L",
            email: new Email("e@f.com"),
            phone: "111",
            status: TenantStatus.TRIAL,
            createdAt: DateTime.UtcNow.AddMinutes(-10),
            updatedAt: DateTime.UtcNow.AddMinutes(-10)
        );

        // Act
        tenant.UpdateContactInfo(new Email("new@mail.com"), "222");

        // Assert
        tenant.Email.Value.Should().Be("new@mail.com");
        tenant.Phone.Should().Be("222");
        tenant.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateStatus_Should_Change_Status_And_UpdatedAt()
    {
        // Arrange
        var tenant = TenantDomainEntity.Create(
            id: 1,
            code: "C",
            subDomain: "c",
            displayName: "D",
            legalName: "L",
            email: new Email("e@f.com"),
            phone: "1",
            status: TenantStatus.TRIAL,
            createdAt: DateTime.UtcNow.AddMinutes(-10),
            updatedAt: DateTime.UtcNow.AddMinutes(-10)
        );

        // Act
        tenant.UpdateStatus(TenantStatus.ACTIVE);

        // Assert
        tenant.Status.Should().Be(TenantStatus.ACTIVE);
        tenant.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
