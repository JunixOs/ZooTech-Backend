namespace ZooTech.Application.Common.Gateway.Time;

public interface IDateTimeProvider
{
    DateTime ServerNow { get; }
    DateTime UtcNow { get; }
}
