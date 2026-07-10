using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class InactiveUserException : AppApplicationException
    {
        public InactiveUserException(
            ScopeName scopeName,
            ModuleName? moduleName = null
        ) : base(
            "INACTIVE_USER_ERROR",
            ErrorType.Cancelled,
            scopeName,
            "the user are inactive",
            moduleName
        )
        {
        }
    }
}