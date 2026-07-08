using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.InterfaceAdapters.Exceptions
{
    public abstract class AppInterfaceAdaptersException : AppDomainException
    {
        protected AppInterfaceAdaptersException(
            string errorCode,
            ErrorType errorType,
            ScopeName scopeName,
            string message,
            ModuleName? moduleName = null,
            List<string>? details = null
        ) : base(
            errorCode, 
            errorType,
            scopeName, 
            message, 
            moduleName,
            details
        )
        {
        }
    }
}