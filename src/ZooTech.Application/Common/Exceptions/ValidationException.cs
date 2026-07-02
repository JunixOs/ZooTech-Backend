using System.Net;
using FluentValidation.Results;

namespace ZooTech.Application.Common.Exceptions
{
    public class ValidationException : AppException
    {
        public override int StatusCode => (int)HttpStatusCode.BadRequest;

        public IReadOnlyList<ValidationError> Errors { get; }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : base("APPLICATION_VALIDATION_ERROR", "One or more parameters of the request are invalid or incorrect.", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}").ToList())
        {
            Errors = failures.Select(f => new ValidationError(f.PropertyName, f.ErrorMessage)).ToList();
        }
    }

    public record ValidationError(string PropertyName, string Message);
}
