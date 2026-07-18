using System.Text.Json.Serialization;

namespace ZooTech.InterfaceAdapters.Models
{
    public class ErrorResponseModel
    {
        public ErrorContent Error { get; set; } = default!;
    }

    public class ErrorContent
    {
        [JsonPropertyName("code")]
        public required string ErrorCode { get; set; }
        public required string Message { get; set; }
        public List<string> Details { get; set; } = [];
        public IReadOnlyList<FieldErrorContent> FieldErrors { get; set; } = [];
    }

    public sealed record FieldErrorContent(string Field, string Code, string Message);
}
