using System.Threading;
using System.Threading.Tasks;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public interface IGetActivityStatsInputPort
{
    Task<GetActivityStatsOutput> HandleAsync(GetActivityStatsQuery command, CancellationToken cancellationToken = default);
}
