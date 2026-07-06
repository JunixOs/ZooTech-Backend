namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public interface IGetFecundacionOptionsInputPort
{
    Task<GetFecundacionOptionsOutput> HandleAsync(CancellationToken cancellationToken = default);
}
