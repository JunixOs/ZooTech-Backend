using System.Net;

namespace ZooTech.Application.Common.Exceptions
{
    public class TenantProvisioningException : AppException
    {
        public override int StatusCode => (int)HttpStatusCode.InternalServerError;

        public TenantProvisioningException() : base(
            "TENANT_PROVISIONING_ERORR",
            "No se pudo crear el nuevo Tenant.",
            null
        )
        {
        }
    }
}