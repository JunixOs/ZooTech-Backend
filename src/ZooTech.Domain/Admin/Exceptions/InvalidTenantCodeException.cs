using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.Domain.Admin.Exceptions;

public class InvalidTenantCodeException : AppDomainException
{
    public InvalidTenantCodeException(string message)
        : base(
            "INVALID_TENANT_CODE_ERROR", 
            ErrorType.Validation,
            ScopeName.Domain, 
            message,
            Shared.Enums.ModuleName.Tenancing
        ) { }
}
