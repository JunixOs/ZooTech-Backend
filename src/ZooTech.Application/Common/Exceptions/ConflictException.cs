using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Exceptions;

public class ConflictException : AppApplicationException
{
        public ConflictException(
            ScopeName scopeName,
            ModuleName? moduleName = null,
            List<string>? details = null,
            string? message = null
        ) : base(
            "CONFLICT_ERROR",
            ErrorType.Conflict,
            scopeName,
            message ?? "A conflict occurred while attempting to enter the data",
            moduleName,
            details
        )
        {
        }
}
