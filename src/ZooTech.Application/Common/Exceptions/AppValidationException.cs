using System.Net;

namespace ZooTech.Application.Common.Exceptions
{
    public class AppValidationException : AppException
    {
        public override int StatusCode => (int)HttpStatusCode.BadRequest;

        public AppValidationException(string moduleName, List<string> errors) : base(
            $"APPLICATION_{moduleName.ToUpper()}_VALIDATION_ERROR",
            "One or more parameters of the request are invalid or incorrect.",
            errors
        )
        {
        }
    }
}