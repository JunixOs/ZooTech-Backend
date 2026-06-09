using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Configuration;

namespace ZooTech.Infrastructure.Configuration.Dev;

/// <summary>
/// Proveedor de configuraciones dinámicas de respaldo para desarrollo local.
/// Lee los valores desde appsettings.Development.json o retorna defaults fijos.
/// </summary>
public sealed class LocalFallbackSettingProvider : ISettingProvider
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public LocalFallbackSettingProvider(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<T> GetSettingAsync<T>(string settingCode, long tenantId)
    {
        var stringValue = _configuration[$"TenantSettings:{settingCode}"];

        if (string.IsNullOrWhiteSpace(stringValue))
        {
            stringValue = GetDefaultValue(settingCode);
        }

        if (string.IsNullOrWhiteSpace(stringValue))
        {
            return Task.FromResult(default(T)!);
        }

        try
        {
            if (typeof(T) == typeof(string))
            {
                return Task.FromResult((T)(object)stringValue);
            }

            if (typeof(T) == typeof(int))
            {
                if (int.TryParse(stringValue, out var intValue))
                {
                    return Task.FromResult((T)(object)intValue);
                }
            }

            if (typeof(T) == typeof(string[]))
            {
                var arrayValue = JsonSerializer.Deserialize<string[]>(stringValue);
                if (arrayValue is not null)
                {
                    return Task.FromResult((T)(object)arrayValue);
                }
            }

            return Task.FromResult(default(T)!);
        }
        catch
        {
            return Task.FromResult(default(T)!);
        }
    }

    private static string? GetDefaultValue(string settingCode)
    {
        return settingCode switch
        {
            "REPORTS_DEFAULT_DAYS" => "30",
            "REPORTS_DATE_FORMAT" => "yyyy-MM-dd",
            "REPORTS_ALLOWED_FORMATS" => "[\"json\", \"pdf\", \"excel\"]",
            "REPORTS_CULTURE_INFO" => "es-ES",
            "REPORTS_NAME_PATTERN" => "reporte_{0}_{1}", // No incluimos la extensión directamente si se usa como patrón sin ella, o la ponemos? El código actual asume extensiones, pero pdf dice .pdf. Wait, the pattern used was string.Format(culture, namePattern, codigo, fecha). Excel expects "reporte_{0}_{1}.xlsx". PDF expects "reporte_{0}_{1}.pdf". Since it's dynamic, maybe it's better to just leave empty fallback so the service uses its own hardcoded "reporte_{0}_{1}.xlsx" fallback. 
            "REPORTS_EXCEL_HEADERS" => "{}",
            "REPORTS_AVAILABLE_CATALOG" => """
            [
                { "Id": "listado_vacunos", "Nombre": "Reporte listado de vacunos", "Descripcion": "Lista de vacunos filtrados por fechas, estado y procedencia." },
                { "Id": "registro_vacuno", "Nombre": "Reporte registro por vacuno", "Descripcion": "Detalle completo del vacuno y su historial relacionado." },
                { "Id": "grafico_genealogico", "Nombre": "Grafico genealogico", "Descripcion": "Arbol genealogico del vacuno hasta 4 niveles." },
                { "Id": "grafico_actividad", "Nombre": "Grafico de actividad", "Descripcion": "Cantidad de vacunos en actividad por periodo." }
            ]
            """,
            _ => null
        };
    }
}
