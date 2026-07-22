using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Domain.Shared.Exceptions
{
    public class NullEmailException : AppDomainException
    {
        public NullEmailException(string message)
            : base(
                "NULL_EMAIL_ERROR", 
                ErrorType.Validation,
                ScopeName.Domain, 
                message
            ) {}
    }
}