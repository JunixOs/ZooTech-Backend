using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.Domain.Admin.Exceptions;

public class InvalidTenantCodeException : DomainException
{
    public InvalidTenantCodeException(string code)
        : base($"Tenant code '{code}' is invalid. It must be alphanumeric with hyphens only and max 50 chars.") { }
}
