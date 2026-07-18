using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class DatabasePermissionException : AppInfrastructureException
    {
        public DatabasePermissionException(
            ModuleName? moduleName = null,
            string? message = null
        ) : base(
            "DATABASE_PERMISSION_EXCEPTION",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            message ?? "the user does not have permission to create databases.",
            moduleName
        )
        {
        }
    }
}