using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class ValidationException : AppApplicationException
    {
        public ValidationException(
            string moduleName, 
            List<string> errors
        ) : base(
            $"APPLICATION_{moduleName.ToUpper()}_VALIDATION_ERROR",
            ErrorType.Validation,
            "One or more parameters of the request are invalid or incorrect.",
            errors
        )
        {
        }
    }
}