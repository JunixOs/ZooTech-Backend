using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Exceptions
{
    public class UndefinedConfigurationValue : AppInfrastructureException
    {
        public UndefinedConfigurationValue(
            ModuleName? moduleName = null,
            string? message = null
        ) : base(
            "UNDEFINED_CONFIGURATION_VALUE_ERROR",
            ErrorType.Infrastructure,
            ScopeName.Infrastructure,
            message ?? "One or more values of the application configuration are Undefined.",
            moduleName
        )
        {
        }
    }
}