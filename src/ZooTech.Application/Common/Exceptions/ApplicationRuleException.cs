namespace ZooTech.Application.Common.Exceptions;

public class ApplicationRuleException : Exception
{
    public ApplicationRuleException(
        string code,
        string message,
        IReadOnlyCollection<ApplicationErrorDetail>? details = null,
        int statusCode = 400)
        : base(message)
    {
        Code = code;
        Details = details ?? Array.Empty<ApplicationErrorDetail>();
        StatusCode = statusCode;
    }

    public string Code { get; }

    public IReadOnlyCollection<ApplicationErrorDetail> Details { get; }

    public int StatusCode { get; }
}
