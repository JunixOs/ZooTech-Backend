using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class ValidationException : AppApplicationException
    {
        public ValidationException(
            List<string> errors,
            ScopeName scopeName,
            ModuleName? moduleName = null 
        ) : base(
            "VALIDATION_ERROR",
            ErrorType.Validation,
            scopeName,
            "One or more parameters of the request are invalid or incorrect.",
            moduleName,
            errors
        )
        {
        }
    }
}