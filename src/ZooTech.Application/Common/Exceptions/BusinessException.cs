using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions;

public class BusinessException : AppApplicationException
{
    public BusinessException(
        ScopeName scopeName,
        ModuleName? moduleName = null,
        List<string>? details = null,
        string? message = null
    ) : base(
        "BUSSINES_ERROR",
        ErrorType.Conflict,
        scopeName,
        message ?? "A conflict occurred while attempting to enter the data",
        moduleName,
        details
    )
    {
    }
}
