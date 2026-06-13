using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime ServerNow => DateTime.UtcNow;
}
