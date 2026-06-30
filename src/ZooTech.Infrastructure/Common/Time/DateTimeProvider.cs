using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Common.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime ServerNow
    {
        get
        {
            var peruZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, peruZone);
        }
    }
}