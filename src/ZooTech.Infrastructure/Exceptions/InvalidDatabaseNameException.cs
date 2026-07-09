using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class InvalidDatabaseNameException : AppInfrastructureException
    {
        public InvalidDatabaseNameException(
            ModuleName? moduleName = null,
            string? databaseName = ""
        ) : base(
            "INVALID_DATABASE_NAME_EXCEPTION",
            ErrorType.Validation,
            ScopeName.Infrastructure,
            $"provided database \"{databaseName}\" name are invalid",
            moduleName
        )
        {
        }
    }
}