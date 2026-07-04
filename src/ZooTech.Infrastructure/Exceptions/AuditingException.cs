using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class AuditingException : AppInfrastructureException
    {
        public AuditingException(
            ModuleName? moduleName = null,
            string? message = null
        ) : base(
            "AUDITING_EXCEPTION",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            message ?? "The log cannot be saved",
            moduleName
        )
        {
        }
    }
}