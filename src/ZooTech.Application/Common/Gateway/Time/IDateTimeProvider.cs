namespace ZooTech.Application.Common.Gateway.Time;

public interface IDateTimeProvider
{
    DateOnly Today { get; }
}
