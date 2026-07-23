using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Domain.Configuration;

namespace ZooTech.API.IntegrationTests.Support;

internal sealed class TestTenantConfigurationProvider : ITenantConfigurationProvider
{
    private readonly ZooTech.Application.Common.Gateway.Context.ITenantContext _tenantContext;

    public TestTenantConfigurationProvider(ZooTech.Application.Common.Gateway.Context.ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    private static readonly IReadOnlyDictionary<string, object> Values =
        new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            [Settings.Vacunos.VacunosArbolMinNiveles.Code] = 1,
            [Settings.Vacunos.VacunosArbolMaxNiveles.Code] = 4,
            [Settings.Vacunos.VacunosCodigoMaxLength.Code] = 15,
            [Settings.Vacunos.VacunosCodigoUppercaseRequired.Code] = true,
            [Settings.Vacunos.VacunosDefaultFilterDays.Code] = 36500,
            [Settings.Vacunos.VacunosInputMaxLength.Code] = 100,
            [Settings.Vacunos.VacunosObservacionesMaxLength.Code] = 150,
            [Settings.Vacunos.VacunosObservacionesMaxWords.Code] = 30,
            [Settings.Vacunos.VacunosFecundacionObservacionesMaxLength.Code] = 250
        };

    public Task<T> GetSettingAsync<T>(SettingDefinition<T> setting)
    {
        if (setting.Code == "VACUNOS_REPORTE_FORMATOS_DESCARGA")
        {
            var allowedFormats = _tenantContext.Code == "LOSA" ? "excel" : "excel,pdf";
            return Task.FromResult((T)(object)allowedFormats);
        }

        if (!Values.TryGetValue(setting.Code, out var value) || value is not T typedValue)
        {
            throw new InvalidOperationException(
                $"No existe un valor de integracion para el parametro {setting.Code}.");
        }

        return Task.FromResult(typedValue);
    }

    public Task<bool> IsFeatureEnabledAsync(FeatureCode feature) => Task.FromResult(true);

    public Task<bool> IsRuleEnabledAsync(RuleCode rule) => Task.FromResult(true);

    public Task InvalidateTenantAsync() => Task.CompletedTask;
}
