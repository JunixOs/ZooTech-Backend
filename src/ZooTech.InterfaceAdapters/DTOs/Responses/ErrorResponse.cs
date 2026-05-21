namespace ZooTech.InterfaceAdapters.DTOs.Responses;

public sealed class ErrorResponse
{
    public ErrorBody Error { get; set; } = new();

    public static ErrorResponse Create(
        string code,
        string message,
        IEnumerable<ErrorDetail>? details = null)
    {
        return new ErrorResponse
        {
            Error = new ErrorBody
            {
                Code = code,
                Message = message,
                Details = details?.ToArray() ?? []
            }
        };
    }
}

public sealed class ErrorBody
{
    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public IReadOnlyCollection<ErrorDetail> Details { get; set; } = [];
}

public sealed class ErrorDetail
{
    public string Field { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
