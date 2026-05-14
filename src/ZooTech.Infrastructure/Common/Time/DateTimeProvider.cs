using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Common.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime ServerNow
    {
        get
        {
            var serverZone = TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, serverZone);
        }
    }
}