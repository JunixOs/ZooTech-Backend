using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.Infrastructure.Exceptions
{
    public abstract class AppInfrastructureException : AppDomainException
    {
        protected AppInfrastructureException(
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