using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class TenantProvisioningException : AppInfrastructureException
    {

        public TenantProvisioningException() : base(
            "TENANT_PROVISIONING_ERROR",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            "the new tenant cannot be created.",
            Domain.Shared.Enums.ModuleName.Tenancing
        )
        {
        }
    }
}
