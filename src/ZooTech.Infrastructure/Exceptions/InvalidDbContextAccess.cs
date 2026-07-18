using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class InvalidDbContextAccess : AppInfrastructureException
    {
        public InvalidDbContextAccess(
            ModuleName? moduleName = null
        ) : base(
            "INVALID_DB_CONTEXT_ACCESS",
            ErrorType.Unauthorized,
            ScopeName.Infrastructure,
            "all or some values of the TenantContext are empty",
            moduleName
        )
        {
        }
    }
}