using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Time;

public sealed class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}