using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Configuration;

namespace ZooTech.Infrastructure.Configuration;

public class VacunosConfiguration : IVacunosConfiguration
{
    private readonly IConfiguration _configuration;

    public VacunosConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int DefaultFilterDays => _configuration.GetValue<int>("ZooSettings:Vacunos:DefaultFilterDays", 30);
    public int ArbolMinNiveles => _configuration.GetValue<int>("ZooSettings:Vacunos:ArbolMinNiveles", 1);
    public int ArbolMaxNiveles => _configuration.GetValue<int>("ZooSettings:Vacunos:ArbolMaxNiveles", 4);
}
