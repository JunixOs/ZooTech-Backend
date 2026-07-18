using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class ValidationException : AppApplicationException, IFieldValidationException
    {
        public IReadOnlyList<FieldValidationError> FieldErrors { get; }

        public ValidationException(
            List<string> errors,
            ScopeName scopeName,
            ModuleName? moduleName = null,
            IReadOnlyList<FieldValidationError>? fieldErrors = null,
            string? message = null
        ) : base(
            "VALIDATION_ERROR",
            ErrorType.Validation,
            scopeName,
            message ?? "One or more parameters of the request are invalid or incorrect.",
            moduleName,
            errors
        )
        {
            FieldErrors = fieldErrors ?? [];
        }
    }
}
