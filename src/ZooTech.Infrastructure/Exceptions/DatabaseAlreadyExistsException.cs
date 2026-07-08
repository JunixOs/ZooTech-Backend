using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class DatabaseAlreadyExistsException : AppInfrastructureException
    {
        public DatabaseAlreadyExistsException(
            ModuleName? moduleName = null,
            string? databaseName = ""
        ) : base(
            "DATABASE_ALREADY_EXISTS_EXCEPTION",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            $"the database [{databaseName}] already exists.",
            moduleName
        )
        {
        }
    }
}