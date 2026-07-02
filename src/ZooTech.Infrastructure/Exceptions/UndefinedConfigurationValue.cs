using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.Infrastructure.Exceptions
{
    public class UndefinedConfigurationValue : AppDomainException
    {
        public UndefinedConfigurationValue() : base(
            "INFRASTRUCTURE_UNDEFINED_CONFIGURATION_VALUE_ERROR",
            ErrorType.Infrastructure,
            "One or more values of the application configuration are Undefined."
        )
        {
        }

        public UndefinedConfigurationValue(Exception innerException) : base(
            "INFRASTRUCTURE_UNDEFINED_CONFIGURATION_VALUE_ERROR",
            ErrorType.Infrastructure,
            "One or more values of the application configuration are Undefined."
        )
        {
        }
    }
}