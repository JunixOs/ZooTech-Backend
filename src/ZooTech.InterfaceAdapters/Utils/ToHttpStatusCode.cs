using System.Net;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.InterfaceAdapters.Utils
{
    public static class ToHttpStatusCode
    {
        public static HttpStatusCode Convert(this ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => HttpStatusCode.BadRequest,
                ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
                ErrorType.Forbidden => HttpStatusCode.Forbidden,
                ErrorType.NotFound => HttpStatusCode.NotFound,
                ErrorType.Conflict => HttpStatusCode.Conflict,
                ErrorType.External => HttpStatusCode.BadGateway,
                ErrorType.Infrastructure => HttpStatusCode.InternalServerError,
                ErrorType.Unexpected => HttpStatusCode.InternalServerError,

                _ => throw new ArgumentOutOfRangeException(nameof(errorType), errorType, null)
            };
        }
    }
}