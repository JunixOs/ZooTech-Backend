using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class DatabaseConnectionException : AppInfrastructureException
    {
        public DatabaseConnectionException(
            string databaseName,
            ModuleName? moduleName = null
        ) : base(
            "DATABASE_CONNECTION_ERROR",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            $"a connection to the database [{databaseName}] could not be established.",
            moduleName
        )
        {
        }
    }
}