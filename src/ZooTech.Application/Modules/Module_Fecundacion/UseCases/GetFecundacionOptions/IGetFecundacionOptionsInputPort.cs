using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public interface IGetFecundacionOptionsInputPort
{
    Task<GetFecundacionOptionsOutput> HandleAsync(
        EmptyCommand emptyCommand,
        CancellationToken cancellationToken = default
    );
}
