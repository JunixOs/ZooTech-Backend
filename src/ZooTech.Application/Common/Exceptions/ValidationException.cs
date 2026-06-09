using System.Net;

namespace ZooTech.Application.Common.Exceptions
{
    public class ValidationException : AppException
    {
        public override int StatusCode => (int)HttpStatusCode.BadRequest;

        public ValidationException(
            List<string>? details = null
        ) : base(
            "VALIDATION_ERROR", 
            "Errores de validación", 
            details
        )
        {
        }
    }
}