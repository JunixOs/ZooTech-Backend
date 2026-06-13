namespace ZooTech.Application.Common.Configuration;

public interface IVacunosConfiguration
{
    int DefaultFilterDays { get; }
    int ArbolMinNiveles { get; }
    int ArbolMaxNiveles { get; }
}
