namespace ZooTech.Application.Common.Gateway.Configuration;

/// <summary>
/// Proveedor de configuraciones dinámicas multi-tenant.
/// </summary>
public interface ISettingProvider
{
    /// <summary>
    /// Obtiene una configuración tipada desde la fuente dinámica de forma asíncrona.
    /// </summary>
    /// <typeparam name="T">El tipo esperado de la configuración (e.g. int, string[], objetos complejos).</typeparam>
    /// <param name="settingCode">El código de la configuración (e.g. "REPORTS_DEFAULT_DAYS").</param>
    /// <param name="tenantId">El identificador único del tenant.</param>
    /// <returns>El valor tipado de la configuración.</returns>
    Task<T> GetSettingAsync<T>(string settingCode, long tenantId);
}
