using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Domain.Shared.Exceptions;

public abstract class AppDomainException : Exception
{
    public string ErrorCode { get; private set; }
    public ErrorType ErrorType { get; private set; }
    public List<string> Details { get; private set; }

    protected AppDomainException(
        string errorCode, 
        ErrorType errorType, 
        string message,
        List<string>? details = null
    ) : base(message)
    {
        ErrorCode = errorCode;
        ErrorType = errorType;

        Details = details ?? [];
    }
    protected AppDomainException(
        string errorCode, 
        ErrorType errorType, 
        string message, 
        Exception inner,
        List<string>? details = null
    ) : base(message, inner)
    {
        ErrorCode = errorCode;
        ErrorType = errorType;

        Details = details ?? [];
    }
}
