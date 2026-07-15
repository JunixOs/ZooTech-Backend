using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZooTech.InterfaceAdapters.Utils;

public sealed class SqlDateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string SqlFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
    private static readonly TimeZoneInfo PeruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return reader.GetDateTime();
        }

        var rawValue = reader.GetString();
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return default;
        }

        if (DateTime.TryParseExact(rawValue, SqlFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var sqlDateTime))
        {
            return DateTime.SpecifyKind(sqlDateTime, DateTimeKind.Unspecified);
        }

        if (DateTimeOffset.TryParse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var valueWithOffset))
        {
            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(valueWithOffset, PeruTimeZone).DateTime, DateTimeKind.Unspecified);
        }

        var parsed = DateTime.Parse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        return DateTime.SpecifyKind(parsed, DateTimeKind.Unspecified);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(SqlFormat, CultureInfo.InvariantCulture));
}
