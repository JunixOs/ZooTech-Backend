using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.Application.Common.Exceptions
{
    public abstract class AppApplicationException : AppDomainException
    {

        protected AppApplicationException(
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