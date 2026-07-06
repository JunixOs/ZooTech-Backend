using ZooTech.Domain.Shared.Enums;

namespace ZooTech.InterfaceAdapters.Exceptions
{
    public class AlreadyAuthenticatedException : AppInterfaceAdaptersException
    {
        public AlreadyAuthenticatedException(
            string? message = null,
            ModuleName? moduleName = null
        ) : base(
            "ALREADY_AUTHENTICATED_ERROR", 
            ErrorType.Conflict,
            ScopeName.Interface_Adapters, 
            message ?? "access denied; an active session already exists.", 
            moduleName
        )
        {
        }
    }
}