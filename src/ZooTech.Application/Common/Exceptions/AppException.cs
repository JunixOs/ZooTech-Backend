namespace ZooTech.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public string Code { get; }
        
        public List<string> Details { get; }

        protected AppException(
            string code,
            string message,
            List<string>? details = null
        ) : base(message)
        {
            Code = code;

            Details = details ?? [];
        }

        public abstract int StatusCode { get; }
    }

    public class ErrorResponse
    {
        public ErrorContent Error { get; set; } = default!;
    }

    public class ErrorContent
    {
        public required string Code { get; set; }
        public required string Message { get; set; }
        public List<string> Details { get; set; } = [];
    }
}