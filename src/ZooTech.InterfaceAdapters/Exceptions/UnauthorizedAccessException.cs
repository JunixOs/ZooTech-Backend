using ZooTech.Domain.Shared.Enums;

namespace ZooTech.InterfaceAdapters.Exceptions
{
    public class UnauthorizedAccessException : AppInterfaceAdaptersException
    {
        public UnauthorizedAccessException(
            ModuleName? moduleName = null
        ) : base(
            "UNAUTHORIZED_ACCESS",
            ErrorType.Unauthorized,
            ScopeName.Interface_Adapters,
            "you are not authorized to access this resource",
            moduleName
        )
        {
        }
    }
}