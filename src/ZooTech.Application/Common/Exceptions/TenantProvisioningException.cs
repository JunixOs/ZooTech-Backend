using System.Net;

namespace ZooTech.Application.Common.Exceptions
{
    public class TenantProvisioningException : AppException
    {
        public override int StatusCode => (int)HttpStatusCode.InternalServerError;

        public TenantProvisioningException() : base(
            "APPLICATION_TENANT_TENANT_PROVISIONING_ERROR",
            "the new tenant cannot be created."
        )
        {
        }

        public TenantProvisioningException(Exception innerException)
            : base("TENANT_PROVISIONING_ERORR", "the new tenant cannot be created.")
        {
        }
    }
}
