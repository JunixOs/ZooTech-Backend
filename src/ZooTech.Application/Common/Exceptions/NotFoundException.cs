using System.Net;

namespace ZooTech.Application.Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public override int StatusCode => (int)HttpStatusCode.NotFound;

        public NotFoundException(
            string moduleName
        ) : base($"APPLICATION_{moduleName.ToUpper()}_NOT_FOUND_ERROR" , "Some elements cannot be founded.")
        {
        }
    }
}