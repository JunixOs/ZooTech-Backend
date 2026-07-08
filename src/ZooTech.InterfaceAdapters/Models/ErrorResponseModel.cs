namespace ZooTech.InterfaceAdapters.Models
{
    public class ErrorResponseModel
    {
        public ErrorContent Error { get; set; } = default!;
    }

    public class ErrorContent
    {
        public required string ErrorCode { get; set; }
        public required string Message { get; set; }
        public List<string> Details { get; set; } = [];
    }
}