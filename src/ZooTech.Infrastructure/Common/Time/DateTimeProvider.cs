using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Common.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime ServerNow =>
        DateTimeOffset.UtcNow
            .ToOffset(TimeSpan.FromHours(-7))
            .DateTime;
}
