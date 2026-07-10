using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class InvalidCredentialsException : AppApplicationException
    {
        public InvalidCredentialsException(
            ScopeName scopeName,
            ModuleName? moduleName = null
        ) : base(
            "INVALID_CREDENTIALS_ERROR",
            ErrorType.Validation,
            scopeName,
            "the credentials are invalid",
            moduleName
        )
        {
        }
    }
}