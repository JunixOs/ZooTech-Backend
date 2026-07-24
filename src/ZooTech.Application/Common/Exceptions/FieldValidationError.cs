namespace ZooTech.Application.Common.Exceptions;

public sealed record FieldValidationError(
    string Field,
    string Code,
    string Message);

public interface IFieldValidationException
{
    IReadOnlyList<FieldValidationError> FieldErrors { get; }
}
