using System.Threading;
using System.Threading.Tasks;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GetReporteDiario;

public interface IGetReporteDiarioInputPort
{
    Task<GetReporteDiarioOutput> HandleAsync(GetReporteDiarioQuery query, CancellationToken cancellationToken);
}
