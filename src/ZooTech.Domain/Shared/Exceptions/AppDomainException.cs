using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Domain.Shared.Exceptions;

public abstract class AppDomainException : Exception
{
    public string ErrorCode { get; private set; }
    public string CompleteErrorCode { get; private set; }

    public ErrorType ErrorType { get; private set; }
    public ScopeName ScopeName { get; private set; }

    public ModuleName? ModuleName { get; private set; }

    public List<string> Details { get; private set; }

    protected AppDomainException(
        string errorCode, 
        ErrorType errorType,
        ScopeName scopeName,
        string message,
        ModuleName? moduleName = null,
        List<string>? details = null
    ) : base(message)
    {
        ErrorCode = errorCode;
        ErrorType = errorType;
        ScopeName = scopeName;

        ModuleName = moduleName;

        CompleteErrorCode = ModuleName is null
            ? ScopeName.ToString().ToUpper()
            : $"{ScopeName.ToString().ToUpper()}_{ModuleName?.ToString().ToUpper()}";
        CompleteErrorCode = $"{CompleteErrorCode}_{errorCode.ToUpper()}";

        Details = details ?? [];
    }
    protected AppDomainException(
        string errorCode, 
        ErrorType errorType,
        ScopeName scopeName,
        string message,
        Exception inner,
        ModuleName? moduleName = null,
        List<string>? details = null
    ) : base(message, inner)
    {
        ErrorCode = errorCode;
        ErrorType = errorType;
        ScopeName = scopeName;

        ModuleName = moduleName;

        CompleteErrorCode = ModuleName is null
            ? ScopeName.ToString().ToUpper()
            : $"{ScopeName.ToString().ToUpper()}_{ModuleName?.ToString().ToUpper()}";
        CompleteErrorCode = $"{CompleteErrorCode}_{errorCode.ToUpper()}";

        Details = details ?? [];
    }
}
