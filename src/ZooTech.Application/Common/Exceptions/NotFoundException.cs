using System.Net;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions
{
    public class NotFoundException : AppApplicationException
    {
        public NotFoundException(
            ScopeName scopeName,
            ModuleName? moduleName = null
        ) : base(
            "NOT_FOUND_ERROR",
            ErrorType.NotFound,
            scopeName,
            "Some elements cannot be founded.",
            moduleName
        )
        {
        }
    }
}