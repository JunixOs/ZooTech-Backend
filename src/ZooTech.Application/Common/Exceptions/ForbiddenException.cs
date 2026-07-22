using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class ForbiddenException : AppApplicationException
    {
        public ForbiddenException(
            ScopeName scopeName,
            ModuleName? moduleName = null
        ) : base(
            "FORBIDDEN_ERROR",
            ErrorType.Forbidden,
            scopeName,
            "You do not have permission to access this resource.",
            moduleName
        )
        {
        }
    }
}
