namespace ZooTech.Domain.Shared.Exceptions
{
    public class NullEmailException : AppDomainException
    {
        public NullEmailException(string message)
            : base($"DOMAIN_ENUM_NULL_EMAIL_ERROR", message) {}
    }
}