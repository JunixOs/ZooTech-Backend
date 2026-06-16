using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;

namespace ZooTech.Infrastructure.Parametrization;

public sealed class TenantConfigurationRepository : ITenantConfigurationRepository
{
    private readonly string _connectionString;

    public TenantConfigurationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TenantCatalogConnection")
            ?? throw new InvalidOperationException("TenantCatalogConnection is not configured.");
    }

    public async Task<TenantConfiguration> LoadTenantConfigAsync(int tenantId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var settings = new Dictionary<string, string>();
        var features = new HashSet<string>();
        var rules = new HashSet<string>();

        using (var cmd = new SqlCommand(SettingsQuery, connection))
        {
            cmd.Parameters.AddWithValue("@tenantId", tenantId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var code = reader.GetString(0);
                var value = reader.IsDBNull(1) ? null : reader.GetString(1);
                settings[code] = value!;
            }
        }

        using (var cmd = new SqlCommand(FeaturesQuery, connection))
        {
            cmd.Parameters.AddWithValue("@tenantId", tenantId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                features.Add(reader.GetString(0));
            }
        }

        using (var cmd = new SqlCommand(RulesQuery, connection))
        {
            cmd.Parameters.AddWithValue("@tenantId", tenantId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rules.Add(reader.GetString(0));
            }
        }

        return new TenantConfiguration
        {
            TenantId = tenantId,
            Settings = settings,
            EnabledFeatures = features,
            EnabledRules = rules,
            LoadedAt = DateTime.UtcNow
        };
    }

    private const string SettingsQuery = """
        SELECT
            sd.code,
            COALESCE(sv.value, sd.default_value) AS value
        FROM setting_definitions sd
        LEFT JOIN setting_groups sg ON sd.setting_group_id = sg.id
        LEFT JOIN setting_values sv
            ON sv.setting_definition_id = sd.id
            AND sv.tenant_id = @tenantId
            AND sv.actor_type = 'TENANT'
            AND sv.actor_id IS NULL
            AND sv.deleted_at IS NULL
        WHERE sd.is_active = 1
          AND sd.deleted_at IS NULL
          AND sg.is_active = 1
          AND sg.deleted_at IS NULL;
        """;

    private const string FeaturesQuery = """
        SELECT f.code
        FROM features f
        INNER JOIN tenant_features tf
            ON tf.feature_id = f.id
            AND tf.tenant_id = @tenantId
        WHERE f.is_active = 1
          AND f.deleted_at IS NULL
          AND tf.is_enabled = 1;
        """;

    private const string RulesQuery = """
        SELECT rd.code
        FROM rule_definitions rd
        INNER JOIN tenant_business_rules tbr
            ON tbr.rule_definition_id = rd.id
            AND tbr.tenant_id = @tenantId
        WHERE rd.is_active = 1
          AND rd.deleted_at IS NULL
          AND tbr.is_active = 1;
        """;
}
