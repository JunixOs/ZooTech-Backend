using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Domain.Configuration;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Common;

public sealed class FecundacionObservationPolicyTests
{
    [Fact]
    public async Task ValidateAsync_DebeAplicarLimiteConfiguradoPorTenant()
    {
        var tenantConLimiteCorto = CreateSut(10);
        var tenantConLimiteAmplio = CreateSut(20);
        const string observaciones = "12345678901";

        var action = () => tenantConLimiteCorto.ValidateAsync(observaciones);

        var exception = await action.Should().ThrowAsync<ValidationException>();
        exception.Which.FieldErrors.Should().ContainSingle(error =>
            error.Field == "observacionesVeterinarias" &&
            error.Code == "FECUNDACION-OBSERVACIONES_VETERINARIAS-MAX_LENGTH");
        await tenantConLimiteAmplio.ValidateAsync(observaciones);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateAsync_DebeAceptarObservacionesVacias(string? observaciones)
    {
        var sut = CreateSut(10);

        var action = () => sut.ValidateAsync(observaciones);

        await action.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(251)]
    public async Task ValidateAsync_DebeRechazarConfiguracionFueraDelLimiteFisico(int maxLength)
    {
        var sut = CreateSut(maxLength);

        var action = () => sut.ValidateAsync("Observación");

        var exception = await action.Should().ThrowAsync<ValidationException>();
        exception.Which.FieldErrors.Should().ContainSingle(error =>
            error.Field == "configuracion" &&
            error.Code == "FECUNDACION-CONFIG-OBSERVACIONES_MAX_LENGTH-INVALID");
    }

    private static FecundacionObservationPolicy CreateSut(int maxLength)
    {
        var configurationProvider = Substitute.For<ITenantConfigurationProvider>();
        configurationProvider
            .GetSettingAsync(Settings.Vacunos.VacunosFecundacionObservacionesMaxLength)
            .Returns(maxLength);
        return new FecundacionObservationPolicy(configurationProvider);
    }
}
