namespace ZooTech.Domain.Exceptions;

public class InvalidTenantCodeException : DomainException
{
    public InvalidTenantCodeException(string code)
        : base($"Tenant code '{code}' is invalid. It must be alphanumeric with hyphens only and max 50 chars.") { }
}
