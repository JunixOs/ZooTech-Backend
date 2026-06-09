using System;
using FluentAssertions;
using Xunit;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;

namespace ZooTech.Domain.UnitTests.Entities;

public class TenantDomainEntityTests
{
    [Fact]
    public void Create_ConParametrosValidosYFechaDeCreacionNula_DebeCrearEntidadConStatusParseadoYFechaActual()
    {
        // Arrange
        long id = 1;
        string code = "TENANT01";
        string subDomain = "tenant01";
        string displayName = "Tenant 01";
        string legalName = "Tenant 01 S.A.C.";
        string email = "contacto@tenant01.com";
        string phone = "999888777";
        string status = "ACTIVE";
        DateTime? createdAt = null;
        DateTime updatedAt = new DateTime(2026, 1, 1);

        // Act
        var result = TenantDomainEntity.Create(
            id,
            code,
            subDomain,
            displayName,
            legalName,
            email,
            phone,
            status,
            createdAt,
            updatedAt
        );

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Code.Should().Be(code);
        result.SubDomain.Should().Be(subDomain);
        result.DisplayName.Should().Be(displayName);
        result.LegalName.Should().Be(legalName);
        result.Email.Should().Be(email);
        result.Phone.Should().Be(phone);
        result.Status.Should().Be(TenantStatus.ACTIVE);
        
        // Verifica que se haya asignado la fecha actual porque se pasó null
        result.CreatedAt.Date.Should().Be(DateTime.Now.Date); 
        result.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void Create_ConFechaDeCreacionEspecifica_DebeUsarEsaFecha()
    {
        // Arrange
        long id = 2;
        string code = "TENANT02";
        string subDomain = "tenant02";
        string displayName = "Tenant 02";
        string legalName = "Tenant 02 S.A.C.";
        string email = "contacto@tenant02.com";
        string phone = "999888777";
        string status = "TRIAL";
        DateTime? createdAt = new DateTime(2025, 12, 31);
        DateTime updatedAt = new DateTime(2026, 1, 1);

        // Act
        var result = TenantDomainEntity.Create(
            id,
            code,
            subDomain,
            displayName,
            legalName,
            email,
            phone,
            status,
            createdAt,
            updatedAt
        );

        // Assert
        result.Status.Should().Be(TenantStatus.TRIAL);
        result.CreatedAt.Should().Be(new DateTime(2025, 12, 31));
    }

    [Fact]
    public void Create_ConStatusInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        long id = 3;
        string code = "TENANT03";
        string subDomain = "tenant03";
        string displayName = "Tenant 03";
        string legalName = "Tenant 03 S.A.C.";
        string email = "contacto@tenant03.com";
        string phone = "999888777";
        string status = "ESTADO_INEXISTENTE";
        DateTime? createdAt = null;
        DateTime updatedAt = DateTime.Now;

        // Act
        Action act = () => TenantDomainEntity.Create(
            id,
            code,
            subDomain,
            displayName,
            legalName,
            email,
            phone,
            status,
            createdAt,
            updatedAt
        );

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Requested value*was not found.");
    }
}
