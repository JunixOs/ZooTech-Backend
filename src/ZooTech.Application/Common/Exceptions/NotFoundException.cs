using System.Net;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class NotFoundException : AppApplicationException
    {
        public NotFoundException(
            string moduleName
        ) : base(
            $"APPLICATION_{moduleName.ToUpper()}_NOT_FOUND_ERROR",
            ErrorType.NotFound,
            "Some elements cannot be founded."
        )
        {
        }
    }
}