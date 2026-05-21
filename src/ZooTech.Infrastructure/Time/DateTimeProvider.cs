using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Time;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
