using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class EmptyTenantContextValues : AppInfrastructureException
    {
        public EmptyTenantContextValues(
            ModuleName? moduleName = null
        ) : base(
            "EMPTY_TENANT_CONTEXT_VALUES_ERROR",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            "all or some values of the TenantContext are empty",
            moduleName
        )
        {
        }
    }
}