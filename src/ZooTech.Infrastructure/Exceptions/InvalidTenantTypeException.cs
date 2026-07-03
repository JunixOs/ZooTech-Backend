using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class InvalidTenantTypeException : AppInfrastructureException
    {
        public InvalidTenantTypeException(
            string actualTenanTypeValue,
            ModuleName? moduleName = null
        ) : base(
            "INVALID_TENANT_TYPE_ERROR",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            $"The value [{actualTenanTypeValue}] is invalid for TenantType",
            moduleName
        )
        {
        }
    }
}