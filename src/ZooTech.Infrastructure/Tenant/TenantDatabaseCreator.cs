using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Infrastructure.Exceptions;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantDatabaseCreator : ITenantDatabaseCreator
    {
        private readonly SqlConnectionStringBuilder _builder;
        private readonly string _appLoginName;

        private static readonly Regex DatabaseRegex =
            new(@"^[A-Za-z0-9_]+$");

        public TenantDatabaseCreator(
            IConfiguration config
        )
        {
            var adminConnection  = config.GetConnectionString("AdminTenantTemplate")
                ?? throw new UndefinedConfigurationValue(
                    ModuleName.Tenancing,
                    message: "Missing configuration: ConnectionStrings:AdminTenantTemplate"
                );
            var appConnection = config.GetConnectionString("TenantTemplate")
                ?? throw new UndefinedConfigurationValue(
                    ModuleName.Tenancing,
                    message: "Missing configuration: ConnectionStrings:TenantTemplate"
                );

            _builder = new SqlConnectionStringBuilder(adminConnection);

            var appBuilder = new SqlConnectionStringBuilder(appConnection);

            _appLoginName = appBuilder.UserID;
        }

        private async Task<SqlConnection> OpenMasterConnectionAsync()
        {
            var builder = new SqlConnectionStringBuilder(_builder.ConnectionString)
            {
                InitialCatalog = "master"
            };

            var connection = new SqlConnection(builder.ConnectionString);

            await connection.OpenAsync();

            return connection;
        }

        private async Task CreateApplicationUserAsync(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder(_builder.ConnectionString)
            {
                InitialCatalog = databaseName
            };

            await using var connection =
                new SqlConnection(builder.ConnectionString);

            await connection.OpenAsync();

            var sql = $"""
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.database_principals
                    WHERE name = @_login
                )
                BEGIN
                    CREATE USER [{_appLoginName}]
                    FOR LOGIN [{_appLoginName}];

                    ALTER ROLE db_owner
                    ADD MEMBER [{_appLoginName}];
                END
                """;

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@_login", _appLoginName);

            await command.ExecuteNonQueryAsync();
        }

        public async Task CreateAsync(string databaseName)
        {
            if (!DatabaseRegex.IsMatch(databaseName))
                throw new InvalidDatabaseNameException(
                    databaseName: databaseName
                );
            
            if (await ExistsAsync(databaseName))
                return;

            var sql = $"CREATE DATABASE [{databaseName}]";

            await using var connection = await OpenMasterConnectionAsync();

            await using var command = new SqlCommand(sql, connection);

            try
            {
                await command.ExecuteNonQueryAsync();

                await CreateApplicationUserAsync(databaseName);
            }
            catch (SqlException ex) when (ex.Number == 262)
            {
                throw new DatabasePermissionException();
            }
            catch (SqlException ex) when (ex.Number == 1801)
            {
                throw new DatabaseAlreadyExistsException(
                    databaseName: databaseName
                );
            }
        }

        public async Task DeleteAsync(string databaseName)
        {
            if (!DatabaseRegex.IsMatch(databaseName))
                throw new InvalidDatabaseNameException(
                    databaseName: databaseName
                );

            if (!await ExistsAsync(databaseName))
                return;

            var sql = $"""
                ALTER DATABASE [{databaseName}]
                SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

                DROP DATABASE [{databaseName}];
                """;

            await using var connection = await OpenMasterConnectionAsync();

            await using var command = new SqlCommand(sql, connection);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) when (ex.Number == 262)
            {
                throw new DatabasePermissionException();
            }
        }

        public async Task<bool> ExistsAsync(string databaseName)
        {
            const string sql = """
                SELECT COUNT(*)
                FROM sys.databases
                WHERE name = @databaseName
                """;

            await using var connection = await OpenMasterConnectionAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add(
            "@databaseName",
            SqlDbType.NVarChar,
            128).Value = databaseName;

            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }
    }
}