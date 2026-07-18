using ZooTech.Application.Common.Exceptions;

namespace ZooTech.Application.Common.Validator;

public interface IValidationErrorDetailsProvider
{
    IReadOnlyList<FieldValidationError> GetFieldErrors(IReadOnlyCollection<string> errorCodes);
}
