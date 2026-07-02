using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.Infrastructure.Exceptions
{
    public class TenantProvisioningException : AppDomainException
    {

        public TenantProvisioningException() : base(
            "INFRASTRUCTURE_TENANT_PROVISIONING_ERROR",
            ErrorType.Infrastructure,
            "the new tenant cannot be created."
        )
        {
        }

        public TenantProvisioningException(Exception innerException)
            : base(
                "APPLICATION_TENANT_PROVISIONING_ERORR",
                ErrorType.Infrastructure, 
                "the new tenant cannot be created."
            )
        {
        }
    }
}
