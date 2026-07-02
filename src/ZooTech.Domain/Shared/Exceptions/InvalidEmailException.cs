using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Domain.Shared.Exceptions
{
    public class InvalidEmailException : AppDomainException
    {
        public InvalidEmailException(string message)
            : base($"DOMAIN_ENUM_INVALID_EMAIL_ERROR", ErrorType.Validation, message) {}
    }
}