using System.Net;

namespace ZooTech.Application.Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public override int StatusCode => (int)HttpStatusCode.NotFound;

        public NotFoundException(string message) : base("NOT_FOUND", message)
        {
        }

        public NotFoundException(
            string code,
            string message
        ) : base(code, message)
        {
        }
    }
}
