using Dapper;
using Microsoft.Data.SqlClient;
using ZooTech.CodeGeneration.Models;

namespace ZooTech.CodeGeneration.Readers;

public sealed class MetadataReader
{
    private readonly string _connectionString;

    public MetadataReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyCollection<SettingMetadata>> GetSettingsAsync()
    {
        const string sql = """
            SELECT
                sg.code AS GroupCode,
                sd.code AS Code,
                sd.data_type AS DataType
            FROM setting_groups sg
            INNER JOIN setting_definitions sd
                ON sd.setting_group_id = sg.id
            WHERE
                sg.is_active = 1
                AND sd.is_active = 1
                AND sg.deleted_at IS NULL
                AND sd.deleted_at IS NULL
            ORDER BY
                sg.code,
                sd.code
            """;

        using var conn = new SqlConnection(_connectionString);

        return (await conn.QueryAsync<SettingMetadata>(sql)).ToList();
    }

    public async Task<IReadOnlyCollection<FeatureMetadata>> GetFeaturesAsync()
    {
        const string sql = """
            SELECT code AS Code
            FROM features
            WHERE
                is_active = 1
                AND deleted_at IS NULL
            ORDER BY code
            """;

        using var conn = new SqlConnection(_connectionString);

        return (await conn.QueryAsync<FeatureMetadata>(sql)).ToList();
    }

    public async Task<IReadOnlyCollection<RuleMetadata>> GetRulesAsync()
    {
        const string sql = """
            SELECT code AS Code
            FROM rule_definitions
            WHERE
                is_active = 1
                AND deleted_at IS NULL
            ORDER BY code
            """;

        using var conn = new SqlConnection(_connectionString);

        return (await conn.QueryAsync<RuleMetadata>(sql)).ToList();
    }
}
