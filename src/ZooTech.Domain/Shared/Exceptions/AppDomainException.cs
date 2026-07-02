namespace ZooTech.Domain.Shared.Exceptions;

public abstract class AppDomainException : Exception
{
    protected string Code { get; private set; }
    protected AppDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
    protected AppDomainException(string code, string message, Exception inner) : base(message, inner)
    {
        Code = code;
    }
}
