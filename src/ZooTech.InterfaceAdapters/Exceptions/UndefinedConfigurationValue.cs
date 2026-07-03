using ZooTech.Domain.Shared.Enums;

namespace ZooTech.InterfaceAdapters.Exceptions
{
    public class UndefinedConfigurationValue : AppInterfaceAdaptersException
    {
        public UndefinedConfigurationValue(
            ModuleName? moduleName = null
        ) : base(
            "UNDEFINED_CONFIGURATION_VALUE_ERROR",
            ErrorType.Infrastructure,
            ScopeName.Interface_Adapters,
            "One or more values of the application configuration are Undefined.",
            moduleName
        )
        {
        }
    }
}